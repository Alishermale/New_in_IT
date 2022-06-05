from socket import socket, AF_INET, SOCK_STREAM
import sys


sock = socket(AF_INET, SOCK_STREAM)
sock.connect(("192.168.90.180", 5000))
data = sys.argv[1]
if len(sys.argv) > 2:
    data += u"\u0017" + sys.argv[2]
    data = data.encode()
sock.send(data)
data = sock.recv(8192)  # читаем ответ от серверного сокета
sock.close()  # закрываем соединение
predict = data.decode()
print(predict)
