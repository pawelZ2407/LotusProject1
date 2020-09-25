from Packet import *


class Player():
    def __init__(self, username='', connection=None):
        self.username = username
        self.id = 0
        self.connection = connection


    def get_packet(self):
        msg = []
        while END_MSG_CHAR not in msg:
            try:
                msg.append(self.connection.recv(1))
            except:
                return None
        return Packet(''.join(msg))

    def send_packet(self, packet):
        msg = packet.msg
        try:
            self.connection.sendall(msg)
        except Exception as e:
            print(e)
