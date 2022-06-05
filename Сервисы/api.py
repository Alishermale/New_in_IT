from flask import Flask
import numpy as np
from flask_restful import Api, Resource
from utils import predict_model, reversed_categories, dict_predict
import json


app = Flask(__name__)
api = Api(app)


class Quote(Resource):
    def get(self, text, kol=None):
        predict = predict_model(text)
        if kol is None:
            return {'text': json.dumps(predict.tolist())}
        if kol == -1:
            predict = [reversed_categories[x - 1] for x in reversed(np.argsort(predict)[0]) if x - 1 != -1]
            return {'text': json.dumps(predict.tolist())}
        if kol == 0:
            predict = reversed_categories[np.argmax(predict)]
        return dict_predict(predict, kol)


api.add_resource(Quote, "/<string:text><int:kol>")
if __name__ == '__main__':
    app.run(debug=True, port=5001)
