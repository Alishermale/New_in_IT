import json
import re
import numpy as np
from tensorflow.keras.preprocessing.text import tokenizer_from_json
from tensorflow.keras.preprocessing.sequence import pad_sequences
from tensorflow.keras.models import load_model


maxSequenceLength = 40
model = load_model('models/my_model/1')

with open('categories.json', 'r') as f:
    categories = json.load(f)
reversed_categories = dict((v, int(k)) for k, v in categories.items())


with open('tokenizer.json') as f:
    data = json.load(f)
    tokenizer = tokenizer_from_json(data)


def clear_text(data):
    data = re.sub(r"[^\w\s]", " ", str(data).lower()) # понижение в нижний регист и удаление знаки препинания
    data = ' '.join(words for words in data.split())
    return data


def prep_text(texts, tokenizer, max_sequence_length):
    text_sequences = tokenizer.texts_to_sequences(texts)
    return pad_sequences(text_sequences, maxlen=max_sequence_length, padding='post')


def prep_data_one_str(text):
    data = clear_text(text)
    return prep_text([data], tokenizer, maxSequenceLength)


def predict_model(data):
    data = prep_data_one_str(data)
    return model.predict(data)


def dict_predict(predict, elementov):
    order = np.argsort(predict)
    result = {}
    for i in range(elementov):
        value_index = order[0][-i - 1]
        result[reversed_categories[value_index]] = predict[0][value_index]
    return dict((str(k), str(v)) for k, v in result.items())
