import json
import numpy as np
from utils import predict_model, reversed_categories, dict_predict
from socketserver import BaseRequestHandler, ThreadingTCPServer


class EchoHandler(BaseRequestHandler):
    def handle(self):
        msg = self.request.recv(1024)
        data = msg.decode().split(u"\u0017")
        predict = predict_model(data[0])
        if len(data) > 1:
            elementov = int(data[1])
            if elementov == 0:
                predict = reversed_categories[np.argmax(predict)]
            elif elementov == -1:
                predict = [reversed_categories[x - 1] for x in reversed(np.argsort(predict)[0]) if x - 1 != -1]
            else:
                predict = dict_predict(predict, elementov)
        else:
            predict = predict.tolist()
        json_data = json.dumps(predict)
        self.request.send(json_data.encode())


if __name__ == '__main__':
    print("Сервер запущен")
    server = ThreadingTCPServer(('', 5000), EchoHandler)
    server.serve_forever()
