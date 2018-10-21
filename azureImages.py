import requests
from io import open as iopen
from urlparse import urlsplit
import itertools
import json
from gtts import gTTS

def requests_image(file_url):
	try:
	    suffix_list = ['jpg', 'gif', 'png', 'tif', 'svg',]
	    file_name =  "photos/" + urlsplit(file_url)[2].split('/')[-1]
	    file_suffix = file_name.split('.')[1]
	    i = requests.get(file_url)
	    with iopen(file_name, 'wb') as file:
	        file.write(i.content)
	    return file_name
	except:
		return None


def getImage(term):

	search_url = "https://api.cognitive.microsoft.com/bing/v7.0/images/search"
	search_term = term
	subscription_key = "248edb5e2bf347f288144b2cbffcb92b"
	headers = {"Ocp-Apim-Subscription-Key" : subscription_key}
	params  = {"q": search_term, "textDecorations":True, "textFormat":"HTML", "safeSearch": "Strict"}
	response = requests.get(search_url, headers=headers, params=params)
	response.raise_for_status()
	search_results = response.json()
	images = search_results["value"]
	return requests_image(images[0]["contentUrl"])

subscription_key = "519a30407f7248dcad9bbff1cecedf74"
text_analytics_base_url = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/"

def reorder(array):
	return sorted(array, key = lambda i: int(i['id']))

def sentiment(text):
	sentiment_api_url = text_analytics_base_url + "sentiment"
	docs = [{'id': str(text.index(x)), 'language': 'en', 'text': x} for x in text]
	documents = {'documents' : docs}
	headers   = {"Ocp-Apim-Subscription-Key": subscription_key}
	response  = requests.post(sentiment_api_url, headers=headers, json=documents)
	sentiments = response.json()
	val = map(lambda x: x['score'], reorder(sentiments["documents"]))
	return val

def keyPhrases(text):
	key_phrase_api_url = text_analytics_base_url + "keyPhrases"
	docs = [{'id': str(text.index(x)), 'language': 'en', 'text': x} for x in text]
	documents = {'documents' : docs}
	headers   = {'Ocp-Apim-Subscription-Key': subscription_key}
	response  = requests.post(key_phrase_api_url, headers=headers, json=documents)
	key_phrases = response.json()
	val = map(lambda x: x['keyPhrases'], reorder(key_phrases["documents"]))
	return val

def analyzeText(text):
	json = {}
	json["originalText"] = text
	json["rooms"] = []
	text = text.split('.')
	for i, (sent, terms) in enumerate(zip(sentiment(text), keyPhrases(text))):
		audioPath = outputAudio(i, text[i])
		images = []
		for phrase in terms:
			photo = getImage(phrase)
			print photo, phrase
			images.append(photo)
		json["rooms"].append({"text": text[i], "sentiment": sent, "associatedTags": terms, "imagePaths": images, "audioPath": audioPath})
		
	return json

def outputAudio(number, sentence):
	tts = gTTS(sentence)
	path = "audio/" + str(number) + '.mp3'
	tts.save(path)
	return path


inputFileName = raw_input('Enter file name: ')
f = open(str(inputFileName), "r")
val = analyzeText(f.read())
print(val)
f = open("map.json", "w")
f.write(json.dumps(val))
