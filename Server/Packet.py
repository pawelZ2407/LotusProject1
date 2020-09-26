# Packet Types
# Packets with number < 0 doesn't require propagation to other players
REGISTER_TYPE = -2
LOGIN_TYPE = -1
NOT_VALID_TYPE = 0
LOGIN_SUCCESSFULY = 1
DISCONNECT_TYPE = 3
PLAYER_MOVEMENT_TYPE = 4
PLAYER_RUN_TYPE = 5
PLAYER_DROP_ITEM_TYPE = 6
PLAYER_GRAB_ITEM_TYPE = 7
PLAYER_BUILD_TYPE = 8
PLAYER_SHOOT_TYPE = 9
PLAYER_SHIELD_TYPE = 10
PLAYER_CHAT_TYPE = 11

# Msg special chars
END_MSG_CHAR = ';'
DELIMITER_CHAR = ','
KEY_VALUE_CHAR = '='

''' All (except the first) packets starts with TYPE=(MSG_TYPE),ID=(USER_ID)
    The first only starts with TYPE and not with ID'''


class Packet():
    def __init__(self, msg=''):
        self.msg = msg
        self.content = self._get_content()
        self.type = self._get_type()
        self.does_propagation = self._does_propagation()

    def is_valid(self):
        return bool(self.type)

    def _get_type(self):
        return self.content.get('TYPE', NOT_VALID_TYPE)

    def _get_content(self):
        # Return dict with content_name/value
        content = {}
        splited_msg = self.msg[:-1].split(DELIMITER_CHAR)
        for item in splited_msg:
            key, value = item.split(KEY_VALUE_CHAR)
            content[key] = value
        return content

    
    def _does_propagation(self):
        #Decides if packet has to propagate to all users'''
        if self.type > 0: return True
        return False
