#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

import sys
sys.path.append( "..\\" )

from PythonCard import dialog, model
from threading import Thread
from socket import *
from Modules.bidder_data_handler import bidder_data_handler as bdh
from Modules.bidder_data_handler_const import *
from BidderDataServer.server_id import CONNECTER_DATA_INPUT
import re

# �s�uport
DEFAULT_PORT	= 5566

# �P���̤j���
MAX_DIGIT			= 5

# �Ȧs��IP�s�ɸ��|
CACHED_IP_PATH	= "cached_ip.ini"

# �Ҧ��`�Ʀr��
Const			= {	"STATICTEXTNAME":						"�ǶR�a�m�W",
							"STATICTEXTCOMPANY":				"���q�W��",
							"STATICTEXTCAREERTITLE":		"¾��",
							"STATICTEXTIDNUMBER":				"������/�@�Ӹ��X",
							"STATICTEXTTEL":						"�q��",
							"STATICTEXTFAX":						"�ǯu",
							"STATICTEXTADDRESS":				"�a�}",
							"STATICTEXTEMAIL":					"E-Mail",
							"STATICTEXTBANK":						"���ӻȦ�/����",
							"STATICTEXTBANKACC":				"�b��",
							"STATICTEXTBANKCONTACT":		"�Ȧ�s���H",
							"STATICTEXTBANKCONTACTTEL":	"�s���q��",
							"STATICTEXTCREDITCARDID":		"�H�Υd���X",
							"STATICTEXTCREDITCARDTYPE":	"�H�Υd���O",
							"STATICTEXTBIDDERID":				"�P��",
							"BUTTONTEXTNEWFILE":				"�s���ɮ�",
							"BUTTONTEXTOPEN":						"�פJ�ɮ�",
							"BUTTONTEXTCONNECT":				"�إ߳s�u",
							"BUTTONTEXTSENDSINGLE":			"�浧�e�X",
							"BUTTONTEXTSENDALL":				"�����e�X",
							"BUTTONTEXTADDBIDDER":			"�s�W�R�a",
							"BUTTONTEXTFIXBIDDER":			"�ץ����",
							"BUTTONTEXTDELBIDDER":			"�R���R�a",
							"NEW_FILE_TITLE":						"�Ыإߤ@�Ӹ����",
							"FILE_RULE":								"�R�a�����(*.txt)|*.txt",
							"NEW_FILE_FAILED":					"�A�����F����ɪ��إ�",
							"NEW_FILE_SUCCESS":					u"���\�إߤF�s������ɡA���|��%s",
							"FILE_NOT_IMPORT":					"<<�A�٨S���פJ�A�������>>",
							"OPEN_FILE_TITLE":					"�ж}�Ҥ@�Ӹ����",
							"OPEN_FILE_FAILED":					"�A�����F����ɪ��}��",
							"OPEN_FILE_SUCCESS":				u"���\�פJ����ɡA���|��%s",
							"OPEN_FILE_NAME":						u"�����<%s>",
							"TEXT_CHK_FAIL_EMPTY":			u"[%s]���i����",
							"BIDDER_ID_LEN_EXCEEDED":		"�P����ƹL�j�A�Эץ�",
							"BIDDER_ID_HAS_FOUR":				"�P�����i�t��""4""�άO""7""",
							"BIDDER_ID_NOT_DIGIT":			"�P�������O�Ʀr�A�Эץ�",
							"WARNING":									"ĵ�i",
							"BIDDER_ID_REPEAT":					u"�P���w�M[%s]����",
							"ADD_BIDDER_SUCCESS":				u"���\�s�W�F�R�a[%s]",
							"NO_SELECTED_BIDDER":				"�п�ܤ@�ӶR�a",
							"FIX_BIDDER_DONE":					u"�w���\�ק�[%s]�����",
							"NO_BIDDER_DATA":						"�d�L�R�a���!! ID = %d",
							"DEL_BIDDER_SUCCESS":				u"���\�R���F�R�a[%s]",
							"WIN_TITLE_TEXT":						"�x�W�@�a���-�v��̸�ƫ��ɨt��",
							"SAVE_FILE_SUCCESS":				"����ɤw�x�s",
							"MAX_DIGIT_NOW":						"�ثe�P���̤j��Ƭ�%d��",
							"UNSAVED_MODS_TITLE":				"���x�s��ƪ��B�z�覡",
							"UNSAVED_MODS_DENY":				"�A�ثe�٦��ק襤�����x�s��ơA�L�k�i�榹�ާ@",
							"UNSAVED_MODS_QUESTION":		"�A�ثe�٦��ק襤�����x�s��ơA�аݧA���U�ӭn....",
							"UNSAVED_MODS_QUIT":				u"���ק�F�A��������",
							"UNSAVED_MODS_RESUME":			"�~��s����",
							"PLZ_ENTER_AUCTION_IP":			"�п�J��椤�ߪ�IP��}",
							"READY_TO_CONNECT":					"�ǳƶ}�l�s�u",
							"DEFAULT_IP":								"127.0.0.1",
							"CONNECT_FAILED":						"�L�k�s�u��[%s]",
							"CONNECT_SUCCESS":					"���\�s�u��[%s]",
							"BIDDER_DATA_SENT":					u"�w�e�X�R�a[%s]�������Ʀܩ�椤��",
							"CONNECT_BEGIN":						u"���b�s�u��[%s]�A�Э@�ߵ���",
						}

# �Ҧ��}�ɫ�~��ϥΪ������ݩ�
ACTIVATE_ATTRS = []
for attr in BIDDER_ATTRS:
	ACTIVATE_ATTRS.append( "TextField" + attr )
ACTIVATE_ATTRS += [ "ButtonConnect", "ButtonAddBidder", "ButtonFixBidder", "ButtonDelBidder" ]

# �Ҧ��s�u��~��ϥΪ������ݩ�
CONNECT_ATTRS = [ "ButtonSendSingle", "ButtonSendAll" ]

# ���T�����������
class class_socket_recv( Thread ):
	def __init__( self, win_ob, socket_ob ):
		Thread.__init__( self )
		
		self.win_ob			= win_ob
		self.socket_ob	= socket_ob
		
		self.last_remain_str = ""
		
	def do_recv_data_input( self, recv_str ):
		print "==="
		print "do_recv_data_input"
		print "len_ori = %d" % len( recv_str )
		# �n�����b�@�_
		recv_str = self.last_remain_str + recv_str
		print "len_now = %d" % len( recv_str )
		# �Ϋʥ]���j�r����}�ʥ]
		recv_str_list = recv_str.split( SEP_PACKET )
		# cache�̫᭱���Ѿl�r��(�@��ӻ��S�W�Lbuffer���|�O"")
		self.last_remain_str = recv_str_list[ -1 ]
		
		if self.last_remain_str != "":
			print "last_remain_str set to %s" % self.last_remain_str
			
		else:
			print "perfect packet"
		
		recv_str_list.pop( -1 )
		for index, recv_str_iter in enumerate( recv_str_list ):
			print "-packet NO.%d, str = %s" % ( index, recv_str_iter )
			self.win_ob.add_msg( recv_str_iter )
			
	def run( self ):
		while True:
			recv_str = self.socket_ob.recv( 1024 )
			self.do_recv_data_input( recv_str )
		
# �s�u���������
class class_socket_connect( Thread ):
	def __init__( self, win_ob ):
		Thread.__init__( self )
		
		self.win_ob			= win_ob;
		self.socket_ob	= socket( AF_INET, SOCK_STREAM )
		
	def run( self ):
		ip, port = self.win_ob.connect_ip.encode( STR_CODE ), DEFAULT_PORT
		try:
			self.socket_ob.connect( ( ip, port ) )
		except:
			self.win_ob.add_msg( Const[ "CONNECT_FAILED" ] % ip )
			return
			
		self.win_ob.add_msg( Const[ "CONNECT_SUCCESS" ] % ip )
		self.win_ob.connected_socket_ob = self.socket_ob
		
		com = self.win_ob.components
		for attr in CONNECT_ATTRS:
			getattr( com, attr ).enabled = True
			
		self.revc_process = class_socket_recv( self.win_ob, self.socket_ob )
		self.revc_process.start()
		
		# ���e���Ҹ�T
		self.socket_ob.send( CONNECTER_DATA_INPUT )

# ������檫��
class MyBackground( model.Background ):
	# �����l��
	def on_initialize( self, event ):
		# ��l�ƩҦ���r ���D
		self.title = Const[ "WIN_TITLE_TEXT" ]
		
		# �R�A��r
		com = self.components
		com.StaticTextName.text						= Const[ "STATICTEXTNAME" ]
		com.StaticTextCompany.text				= Const[ "STATICTEXTCOMPANY" ]
		com.StaticTextCareerTitle.text		= Const[ "STATICTEXTCAREERTITLE" ]
		com.StaticTextIDNumber.text				= Const[ "STATICTEXTIDNUMBER" ]
		com.StaticTextTel.text						= Const[ "STATICTEXTTEL" ]
		com.StaticTextFax.text						= Const[ "STATICTEXTFAX" ]
		com.StaticTextAddress.text				= Const[ "STATICTEXTADDRESS" ]
		com.StaticTextEMail.text					= Const[ "STATICTEXTEMAIL" ]
		com.StaticTextBank.text						= Const[ "STATICTEXTBANK" ]
		com.StaticTextBankAcc.text				= Const[ "STATICTEXTBANKACC" ]
		com.StaticTextBankContact.text		= Const[ "STATICTEXTBANKCONTACT" ]
		com.StaticTextBankContactTel.text	= Const[ "STATICTEXTBANKCONTACTTEL" ]
		com.StaticTextCreditCardID.text		= Const[ "STATICTEXTCREDITCARDID" ]
		com.StaticTextCreditCardType.text	= Const[ "STATICTEXTCREDITCARDTYPE" ]
		com.StaticTextBidderID.text				= Const[ "STATICTEXTBIDDERID" ]
		
		# ���s
		com.ButtonNewFile.label						= Const[ "BUTTONTEXTNEWFILE" ]
		com.ButtonOpen.label							= Const[ "BUTTONTEXTOPEN" ]
		com.ButtonConnect.label						= Const[ "BUTTONTEXTCONNECT" ]
		com.ButtonSendSingle.label				= Const[ "BUTTONTEXTSENDSINGLE" ]
		com.ButtonSendAll.label						= Const[ "BUTTONTEXTSENDALL" ]
		com.ButtonAddBidder.label					= Const[ "BUTTONTEXTADDBIDDER" ]
		com.ButtonFixBidder.label					= Const[ "BUTTONTEXTFIXBIDDER" ]
		com.ButtonDelBidder.label					= Const[ "BUTTONTEXTDELBIDDER" ]
		
		# ����ɦW
		com.StaticTextDataFileName.text		= Const[ "FILE_NOT_IMPORT" ]
		
		# �إ߶R�a��Ƶ��c
		self.bidder_data_map = {}
		
		# �ثe�P���̤j���
		self.max_digit = 0
		
		# �ثe���ɮ׸��|
		self.cur_file_path = ""
		
		# �ثe����ƬO�_�w�x�s
		self.saved_flag = True
		
		# ��������
		# ���ʥ]�������
		self.socket_process = None
		self.connect_ip = u""
		self.connected_socket_ob = None
		
	# Ū���Ȧs��IP
	def load_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "r" )
			
		except:
			return Const[ "DEFAULT_IP" ]
		
		line = file_ob.readline()
		if line[ -1 ] == "\n":
			line = line[ : -1 ]
		
		file_ob.close()
		return line
		
	# �x�s�Ȧs��IP
	def save_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "w" )
			
		except:
			print "open file %s failed" % CACHED_IP_PATH
			return
		
		file_ob.write( self.connect_ip )
		file_ob.close()
		
	# ���U �إ߳s�u ���s
	def on_ButtonConnect_mouseClick( self, event ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_AUCTION_IP" ], Const[ "READY_TO_CONNECT" ], self.load_cached_ip() )
		if not result.accepted:
			return
		
		self.connect_ip = result.text
		self.save_cached_ip()
		
		if self.socket_process:
			del self.socket_process
		self.socket_process = class_socket_connect( self )
		self.socket_process.start()
		
		self.add_msg( Const[ "CONNECT_BEGIN" ] % self.connect_ip )
		
	# ���U �浧��ưe�X ���s
	def on_ButtonSendSingle_mouseClick( self, event ):
		if not self.saved_flag:
			self.add_msg( Const[ "UNSAVED_MODS_DENY" ] )
			return
		
		self.send_bidder_data( self.get_bidder_id_by_selection() )
		
	# ���U �Ҧ���ưe�X ���s
	def on_ButtonSendAll_mouseClick( self, event ):
		if not self.saved_flag:
			self.add_msg( Const[ "UNSAVED_MODS_DENY" ] )
			return
		
		for bidder_id in self.bidder_data_map:
			self.send_bidder_data( bidder_id )
		
	# �e�X���
	def send_bidder_data( self, bidder_id ):
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		bidder_data = self.bidder_data_map[ bidder_id ]
		self.connected_socket_ob.send( bdh.gen_bidder_str_data( bidder_data ) + SEP_PACKET )
		
		self.add_msg( Const[ "BIDDER_DATA_SENT" ] % self.gen_bid_id_plus_name_str( bidder_id ) )
	
	# ���U �s���ɮ� ���s
	def on_ButtonNewFile_mouseClick( self, event ):
		result = dialog.saveFileDialog( title = Const[ "NEW_FILE_TITLE" ], wildcard = Const[ "FILE_RULE" ] )
		if not result.paths:
			self.add_msg( Const[ "NEW_FILE_FAILED" ] )
			return
		
		# �إߤ@�Ӫťդ�r��
		path = result.paths[ 0 ]
		file_ob = open( path, "w" )
		file_ob.close()
		self.add_msg( Const[ "NEW_FILE_SUCCESS" ] % path )
		
		self.run_open_file( path )
	
	# ���U �פJ�ɮ� ���s
	def on_ButtonOpen_mouseClick( self, event ):
		result = dialog.openFileDialog( title = Const[ "OPEN_FILE_TITLE" ], wildcard = Const[ "FILE_RULE" ] )
		if not result.paths:
			self.add_msg( Const[ "OPEN_FILE_FAILED" ] )
			return
		
		self.run_open_file( result.paths[ 0 ] )
	
	# ����פJ�ɮ�
	def run_open_file( self, path ):
		# Ū���J�����ɮ�
		self.cur_file_path = path
		
		# ���m�R�a��Ƶ��c
		self.bidder_data_map, max_digit = bdh.load_data( path )
		
		self.gen_list_ui_by_bidder_data()
		
		# �]�w�̤j���
		self.set_max_digit( max_digit )
		
		# ��s�C�����D
		fname = path[ path.rfind( "\\" ) + 1 : ]
		com = self.components
		com.StaticTextDataFileName.text = Const[ "OPEN_FILE_NAME" ] % fname
		
		# �ҥΦU�Ӥ���
		for attr in ACTIVATE_ATTRS:
			ui_unit = getattr( com, attr )
			ui_unit.enabled = True
			if attr.find( "TextField" ) > -1:
				ui_unit.text = ""
		
		self.update_list_ui()
		self.add_msg( Const[ "OPEN_FILE_SUCCESS" ] % path )
		self.saved_flag = True
	
	# ���U �s�W�ϥΪ� ���s
	def on_ButtonAddBidder_mouseClick( self, event ):
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		index = self.add_bidder_data( bidder_data )
		if index < 0:
			return
		
		self.components.ListBidders.selection = index
		self.add_msg( Const[ "ADD_BIDDER_SUCCESS" ] % self.gen_bid_id_plus_name_str( int( bidder_data[ "BidderID" ] ) ) )
		self.save_bidder_data()
	
	# ���U �s��R�a��� ���s
	def on_ButtonFixBidder_mouseClick( self, event ):
		com = self.components
		
		# �S�����������
		selected_str = com.ListBidders.stringSelection
		if selected_str == "":
			self.add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		# ���o��e�ҿ�ܪ�ID
		bidder_id_str	= bidder_data[ "BidderID" ]
		bidder_id			= int( bidder_id_str )
		bidder_id_now	= self.get_bidder_id_by_selection()
		
		# �p�G�S��P�� ���N�O��§���
		index = com.ListBidders.selection
		if bidder_id == bidder_id_now:
			self.bidder_data_map[ bidder_id ] = bidder_data
			new_title = self.gen_bid_id_plus_name_str( bidder_id )
			# �H���U�@ List����r�]��s�@�U
			com.ListBidders.setString( index, new_title )
			
			self.add_msg( Const[ "FIX_BIDDER_DONE" ] % new_title )
			self.save_bidder_data()
			return
		
		# �p�G��F�P�� ���i�H��{��������
		if bidder_id in self.bidder_data_map:
			self.add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.gen_bid_id_plus_name_str( bidder_id ), True )
			return
			
		# ���R���{�b��
		self.del_bidder_by_index( index )
		
		index = self.add_bidder_data( bidder_data )
		if index < 0:
			return
		
		self.components.ListBidders.selection = index
		self.add_msg( Const[ "FIX_BIDDER_DONE" ] % self.gen_bid_id_plus_name_str( bidder_id ) )
		self.save_bidder_data()
		
	# ���U�R�a�C����
	def on_ListBidders_select( self, event = None ):
		bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		bidder_data = self.bidder_data_map[ bidder_id ]
		for attr in BIDDER_ATTRS:
			getattr( self.components, "TextField" + attr ).text = bidder_data[ attr ]
			
		self.saved_flag = True
		
	# ���U �R���R�a��� ���s
	def on_ButtonDelBidder_mouseClick( self, event ):
		com = self.components
		
		# �S�����������
		index = com.ListBidders.selection
		if index < 0:
			self.add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		bidder_id = self.get_bidder_id_by_selection()
		bid_id_plus_name_str = self.gen_bid_id_plus_name_str( bidder_id )
		
		result = self.del_bidder_by_index( index )
		if not result:
			return
		
		self.update_list_ui()
			
		self.add_msg( Const[ "DEL_BIDDER_SUCCESS" ] % bid_id_plus_name_str )
		self.save_bidder_data()
		
	# ���U [X] ���s
	def on_close( self, event ):
		# �w�g�x�s�F ��������
		if self.saved_flag:
			event.skip()
			return
		
		# ���M������� �~�u������
		result = dialog.singleChoiceDialog( self, Const[ "UNSAVED_MODS_QUESTION" ], Const[ "UNSAVED_MODS_TITLE" ], [ Const[ "UNSAVED_MODS_RESUME" ], Const[ "UNSAVED_MODS_QUIT" ] ] )
		if result.selection == Const[ "UNSAVED_MODS_QUIT" ]:
			event.skip()
			return
	
	# �x�s���
	def save_bidder_data( self ):
		bdh.save_data( self.cur_file_path, self.bidder_data_map )
		
		self.add_msg( Const[ "SAVE_FILE_SUCCESS" ] )
		self.saved_flag = True
	
	# ��s�C��ɭ�
	def update_list_ui( self ):
		# �p�G�٦��ѤU����� ��ܲĤ@�� �_�h�M�ũҦ���r
		com = self.components
		if com.ListBidders.getCount() > 0:
			com.ListBidders.selection = 0;
			self.on_ListBidders_select()
			
		else:
			for attr in BIDDER_ATTRS:
				getattr( com, "TextField" + attr ).text = ""
		
	# �ѯ��ޭȧR�����R�a���
	def del_bidder_by_index( self, index ):
		bidder_id = self.get_bidder_id_by_index( index )
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return False
			
		self.bidder_data_map.pop( bidder_id )
		self.components.ListBidders.delete( index )
		return True
		
	# �Q�ί��ޭȨ��o�P��ID
	def get_bidder_id_by_index( self, index ):
		ui_str = self.components.ListBidders.getString( index )
		if ui_str == "":
			return -1;
		
		return int( ui_str[ : ui_str.find( ":" ) ] )
		
	# �Q�Τ���������o�P��ID
	def get_bidder_id_by_selection( self ):
		index = self.components.ListBidders.selection
		if index < 0:
			return -1;
		
		return self.get_bidder_id_by_index( index )
	
	# �s�W�R�a���
	def add_bidder_data( self, bidder_data ):
		result, bidder_id, is_update_len, new_max_digit = bdh.add_data( bidder_data, self.max_digit, self.bidder_data_map )
		
		# �P�����i����
		if not result:
			self.add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.gen_bid_id_plus_name_str( bidder_id ), True )
			return -1
			
		# ���J�b���T��UI��m
		bidder_id_list = self.bidder_data_map.keys()
		bidder_id_list.sort()
		index = bidder_id_list.index( bidder_id );
		bid_id_plus_name_str = self.gen_bid_id_plus_name_str( bidder_id )
		
		self.components.ListBidders.insertItems( [ bid_id_plus_name_str ], index )
		
		# �̤j�P���Q��F ��s��ӦC��
		if is_update_len:
			# ������
			self.gen_list_ui_by_bidder_data()
			
			self.set_max_digit( new_max_digit )
		return index
	
	# �]�w��e�P���̤j���
	def set_max_digit( self, max_digit ):
		self.max_digit = max_digit
		self.add_msg( Const[ "MAX_DIGIT_NOW" ] % max_digit )
	
	# �Ѭɭ�����r��J�� ���ͶR�a���
	def gen_bidder_data_by_text_field( self ):
		# �@�򥻸���˴�
		com = self.components
		
		# �Ҧ���ƬҤ��i����
		bidder_data = {}
		for attr in BIDDER_ATTRS:
			chk_text = getattr( com, "TextField" + attr ).text
			if attr in NONEMPTY_ATTRS and not len( chk_text ):
				self.add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % getattr( com, "StaticText" + attr ).text )
				return None
			
			bidder_data[ attr ] = chk_text
		
		# �P����Ƥ��i�W�L�̤j��
		bidder_id_str = bidder_data[ "BidderID" ]
		bidder_id_len = len( bidder_id_str )
		if bidder_id_len > MAX_DIGIT:
			self.add_msg( Const[ "BIDDER_ID_LEN_EXCEEDED" ] )
			return None
			
		# �P���L�o�W�h(���i��4, 7)
		re_ob = re.compile( "[47]+" )
		if re_ob.search( bidder_id_str ):
			self.add_msg( Const[ "BIDDER_ID_HAS_FOUR" ] )
			return None
			
		# �P�������O�¼Ʀr
		if not bidder_id_str.isdigit():
			self.add_msg( Const[ "BIDDER_ID_NOT_DIGIT" ] )
			return None
		
		# �ץ��P���r��
		bidder_data[ "BidderID" ] = bdh.complete_str_by_zero( bidder_data[ "BidderID" ], self.max_digit )
		return bidder_data
		
	# �̶R�a��Ʋ��ͬɭ����
	def gen_list_ui_by_bidder_data( self ):
		com = self.components
		
		com.ListBidders.clear()
		bidder_data_ui = []
		# ���Ƨ�
		bidder_id_list = self.bidder_data_map.keys()
		bidder_id_list.sort()
		for bidder_id in bidder_id_list:
			bidder_data_ui.append( self.gen_bid_id_plus_name_str( bidder_id ) )
		
		com.ListBidders.insertItems( bidder_data_ui, 0 )
		
	# ���͵P��:�m�W���r��
	def gen_bid_id_plus_name_str( self, bidder_id ):
		bidder_data = self.bidder_data_map[ bidder_id ]
		return "%s: %s" % ( bidder_data[ "BidderID" ], bidder_data[ "Name" ] )
		
	# �{�Ǭ���
	def add_msg( self, msg, plus_pop = False ):
		if plus_pop:
			dialog.alertDialog( self, msg, Const[ "WARNING" ] )
		self.components.TextAreaLog.appendText( msg + "\n" )

	# �����r��s(�ɤO�k�C�X suck!)
	def on_text_update( self, src_func ):
		self.saved_flag = False
	def on_TextFieldBidderID_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBidderID_textUpdate" )
	def on_TextFieldName_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldName_textUpdate" )
	def on_TextFieldCompany_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCompany_textUpdate" )
	def on_TextFieldCareerTitle_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCareerTitle_textUpdate" )
	def on_TextFieldIDNumber_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldIDNumber_textUpdate" )
	def on_TextFieldTel_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldTel_textUpdate" )
	def on_TextFieldFax_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldFax_textUpdate" )
	def on_TextFieldAddress_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldAddress_textUpdate" )
	def on_TextFieldEMail_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldEMail_textUpdate" )
	def on_TextFieldBank_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBank_textUpdate" )
	def on_TextFieldBankAcc_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankAcc_textUpdate" )
	def on_TextFieldBankContact_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankContact_textUpdate" )
	def on_TextFieldBankContactTel_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankContactTel_textUpdate" )
	def on_TextFieldCreditCardID_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCreditCardID_textUpdate" )
	def on_TextFieldCreditCardType_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCreditCardType_textUpdate" )
		
if __name__ == '__main__':
	app = model.Application(MyBackground)
	app.MainLoop()
