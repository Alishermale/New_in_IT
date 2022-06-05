import json
import sys
import requests
import numpy as np
from utils import prep_data_one_str, dict_predict


with open('categories.json', 'r') as f:
    categories = json.load(f)
reversed_categories = dict((v, int(k)) for k, v in categories.items())


def predict_model(text):
    data = prep_data_one_str(data)
    data = json.dumps({"signature_name": "serving_default", "instances": data.tolist()})
    headers = {"content-type": "application/json"}
    json_response = requests.post('http://localhost:8501/v1/models/my_model:predict', data=data, headers=headers)
    return json.loads(json_response.text)['predictions']


if __name__ == '__main__':
    text = sys.argv[1]
    predict = predict_model(text)
    order = np.argsort(predict)
    print(dict_predict(predict, 6))