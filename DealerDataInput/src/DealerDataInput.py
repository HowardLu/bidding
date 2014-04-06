#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

from PythonCard import dialog, model, EXIF, graphic
import pymongo, sys, time, gridfs
from bson.objectid import ObjectId

ATTR_MAP = {	# ��a�ݩ�
							"Dealer":
								{	"ALL_ATTR":
										(	"Name", "Country", "CardID", "ContractDate", "ContractID", "Tel", "CellPhone", "Fax", "Address", "BankName", "BankAcc", "PostID", "IfDealedInsuranceFee",
											"IfDealedServiceFee", "IfNDealedInsuranceFee", "IfNDealedServiceFee", "IfDealedPictureFee", "IfNDealedPictureFee", "FrameFee", "FireFee", "IdentifyFee"
										),
									"NONEMPTY_ATTR":
										( "Name", "ContractID", "Tel", "Country", "CardID"
										),
									"DIGIT_ATTR":
										( "IfDealedInsuranceFee", "IfDealedServiceFee", "IfNDealedInsuranceFee", "IfNDealedServiceFee", "IfDealedPictureFee", "IfNDealedPictureFee", "FrameFee", "FireFee", "IdentifyFee"
										),
									"KEY_ATTR": "Name", "ListCom": "ListDealer", "CacheData": { "Main": {}, "Index": {} }
								},
							# ��a�ӫ~�ݩ�
							"Item":
								{	"ALL_ATTR":
										( "ItemName", "ItemNum", "Spec", "ReservePrice", "Remain", "ItemPS", "LotNO"
										),
									"NONEMPTY_ATTR":
										( "ItemName"
										),
									"DIGIT_ATTR":
										( "ItemNum", "ReservePrice"
										),
									"KEY_ATTR": "_id", "ListCom": "ListItem", "CacheData": { "Main": {}, "Index": {} }
								},
						}

DEALER_KEY	= ATTR_MAP[ "Dealer" ][ "KEY_ATTR" ]
ITEM_KEY		= ATTR_MAP[ "Item" ][ "KEY_ATTR" ]

ITEM_COM_DATA = { "PackImg": "BitmapCanvasPack", "BuildImg": "BitmapCanvasBuild" }

# �s�uport
CACHED_IP_PATH	= "cached_ip.ini"
DB_CFG_PORT			= 27017

# �L��Key
INVALID_KEY = ""

# �Ҧ��`�Ʀr��
Const			= {	"STATICTEXTNAME":												"�e�U��",
							"STATICTEXTCOUNTRY":										"���y",
							"STATICTEXTCARDID":											"�ҥ󸹽X",
							"STATICTEXTCONTRACTDATE":								"�X�����",
							"STATICTEXTCONTRACTID":									"�X���s��",
							"STATICTEXTEL":													"�q��",
							"STATICTEXTCELLPHONE":									"���",
							"STATICTEXFAX":													"�ǯu",
							"STATICTEXTADDRESS":										"�ԲӦa�}",
							"STATICTEXTBANKNAME":										"�}��Ȧ�",
							"STATICTEXTBANKACC":										"�Ȧ�b��",
							"STATICTEXTPOSTID":											"�l�s",
							"STATICTEXTDEALERLIST":									"��a�C��",
							"STATICTEXTIFDEALED":										"�p��~����G",
							"STATICTEXTIFNDEALED":									"�p��~������G",
							"STATICTEXTIFDEALEDINSURANCEFEE":				"�O�I�O = ���l����",
							"STATICTEXTIFDEALEDSERVICEFEE":					"�A�ȶO = ���l����",
							"STATICTEXTIFNDEALEDINSURANCEFEE":			"�O�I�O = �O�d����",
							"STATICTEXTIFNDEALEDSERVICEFEE":				"�A�ȶO = �O�d����",
							"STATICTEXTPICTUREFEE":									"�Ͽ��O =",
							"STATICTEXTOTHERFEE":										"�䥦�O�ΡG",
							"STATICTEXTFRAMEFEE":										"����/���/�n�X =",
							"STATICTEXTFIREFEE":										"��      ��      �O =",
							"STATICTEXTIDENTIFYFEE":								"ų      �w      �O =",
							"STATICTEXTITEMLIST":										"�e�U���Ъ��C��",
							"BUTTONTEXTADD":												"�s�W",
							"BUTTONTEXTFIX":												"�ץ�",
							"BUTTONTEXTDEL":												"�R��",
							"BUTTONTEXTSET":												"�]�w",
							"STATICTEXTPERCENT":										"%",
							"STATICTEXTSERIAL":											u"�Ǹ�:%s",
							"STATICTEXTNTDPERITEM":									"NTD/��",
							"STATICTEXTITEMNAME":										"���Ъ��W��",
							"STATICTEXTBUILDIMG":										"���ɹϤ�",
							"STATICTEXTPACKIMG":										"�]�˹Ϥ�",
							"STATICTEXTITEMNUM":										"�ƶq",
							"STATICTEXTSPEC":												"�W��(cm)",
							"STATICTEXTRESERVEPRICE":								"�O�d��(NTD)",
							"STATICTEXTRESERVEPRICEPOST":						"�U",
							"STATICTEXTREMAIN":											"�O�s�{��",
							"STATICTEXTITEMPS":											"�Ƶ�",
							"STATICTEXTLOTNO":											"��~�s��",
							"STATICTEXTNOTSET":											"���]�w",
							"STATICTEXTDELIMAGETITLE":							u"�O�_�T�w�N���i�Ϥ����]�w���ť�",
							"STATICTEXTDEL":												u"�]�w���ť�",
							"STATICTEXTKEEP":												u"�O�d�]�w",
							######################################
							"NEW_FILE_TITLE":												"�Ыإߤ@�Ӹ����",
							"FILE_RULE":														"�R�a�����(*.txt)|*.txt",
							"NEW_FILE_FAILED":											"�A�����F����ɪ��إ�",
							"NEW_FILE_SUCCESS":											u"���\�إߤF�s������ɡA���|��%s",
							"FILE_NOT_IMPORT":											"<<�A�|���P��Ʈw�إ߳s�u>>",
							"OPEN_FILE_TITLE":											"�ж}�Ҥ@�i�Ϥ�",
							"OPEN_FILE_FAILED":											"�A�����F����ɪ��}��",
							"OPEN_FILE_SUCCESS":										u"���\�פJ����ɡA���|��%s",
							"OPEN_FILE_NAME":												u"�����<%s>",
							"TEXT_CHK_FAIL_EMPTY":									u"[%s]���i����",
							"TEXT_CHK_FAIL_MUST_DIGIT":							u"[%s]�������Ʀr",
							"TEXT_LOT_NO_NOT_EXIST":								u"��~�s��[%s]���s�b",
							"TEXT_LOT_NO_REPEATED":									u"��~�s��[%s]�w����",
							"BIDDER_ID_LEN_EXCEEDED":								"�P����ƹL�j�A�Эץ�",
							"BIDDER_ID_HAS_FOUR":										"�P�����i�t��""4""�άO""7""",
							"BIDDER_ID_NOT_DIGIT":									"�P�������O�Ʀr�A�Эץ�",
							"WARNING":															"ĵ�i",
							"ERRMSG_NAME_REPEAT":										u"���i�����ƪ��W��",
							"ADD_DEALER_SUCCESS":										u"���\�s�W�F��a[%s]",
							"ADD_ITEM_SUCCESS":											u"���\�s�W�F��~[%s]",
							"NO_SELECTED_BIDDER":										"�п�ܤ@�ӶR�a",
							"NO_SELECTED_ITEM":											"�п�ܤ@�ө�~",
							"FIX_DATA_DONE":												u"�w���\�ק�[%s]�����",
							"NO_DEALER_DATA":												"�d�L�R�a���!! ID = %d",
							"DEL_DATA_SUCCESS":											u"���\�R���F[%s]",
							"WIN_TITLE_TEXT":												"�x�W�@�a���-��a��ƫ��ɨt��",
							"SAVE_FILE_SUCCESS":										"����ɤw�x�s",
							"MAX_DIGIT_NOW":												"�ثe�P���̤j��Ƭ�%d��",
							"UNSAVED_MODS_TITLE":										"���x�s��ƪ��B�z�覡",
							"UNSAVED_MODS_DENY":										"�A�ثe�٦��ק襤�����x�s��ơA�L�k�i�榹�ާ@",
							"UNSAVED_MODS_QUESTION":								"�A�ثe�٦��ק襤�����x�s��ơA�аݧA���U�ӭn....",
							"UNSAVED_MODS_QUIT":										u"���ק�F�A��������",
							"UNSAVED_MODS_RESUME":									"�~��s����",
							"PLZ_ENTER_AUCTION_IP":									"�п�J��椤�ߪ�IP��}",
							"READY_TO_CONNECT":											"�ǳƶ}�l�s�u",
							"DEFAULT_IP":														"127.0.0.1",
							"CONNECT_FAILED":												"�L�k�s�u��[%s]",
							"CONNECT_SUCCESS":											"���\�s�u��[%s]",
							"BIDDER_DATA_SENT":											u"�w�e�X�R�a[%s]�������Ʀܩ�椤��",
							"CONNECT_BEGIN":												u"���b�s�u��[%s]�A�Э@�ߵ���",
							"ERRMSG_DB_CNCT_FAIL":									u"�L�k�s�u���Ʈw( %s, %d )",
							"ERRMSG_DB_CNCT_OK":										u"���\�s�u���Ʈw( %s, %d )",
							"ERRMSG_INVALID_IMG_FILE":							u"�o���O�@�i���Ī��Ϥ���",
							"ERRMSG_IMG_ID_NOT_EXIST":							u"�Ϥ���ƮwID[%s]���s�b",
						}

# ������檫��
class MyBackground( model.Background ):
	# �����l��
	def on_initialize( self, event ):
		# ��l�ƩҦ���r ���D
		self.title = Const[ "WIN_TITLE_TEXT" ]
		
		# �R�A��r
		com = self.components
		com.StaticTextName.text											= Const[ "STATICTEXTNAME" ]
		com.StaticTextCountry.text									= Const[ "STATICTEXTCOUNTRY" ]
		com.StaticTextCardID.text										= Const[ "STATICTEXTCARDID" ]
		com.StaticTextContractDate.text							= Const[ "STATICTEXTCONTRACTDATE" ]
		com.StaticTextContractID.text								= Const[ "STATICTEXTCONTRACTID" ]
		com.StaticTextTel.text											= Const[ "STATICTEXTEL" ]
		com.StaticTextCellPhone.text								= Const[ "STATICTEXTCELLPHONE" ]
		com.StaticTextFax.text											= Const[ "STATICTEXFAX" ]
		com.StaticTextAddress.text									= Const[ "STATICTEXTADDRESS" ]
		com.StaticTextBankName.text									= Const[ "STATICTEXTBANKNAME" ]
		com.StaticTextBankAcc.text									= Const[ "STATICTEXTBANKACC" ]
		com.StaticTextPostID.text										= Const[ "STATICTEXTPOSTID" ]
		com.StaticTextDealerList.text								= Const[ "STATICTEXTDEALERLIST" ]
		com.StaticTextIfDealed.text									= Const[ "STATICTEXTIFDEALED" ]
		com.StaticTextIfNDealed.text								= Const[ "STATICTEXTIFNDEALED" ]
		com.StaticTextIfDealedInsuranceFee.text			= Const[ "STATICTEXTIFDEALEDINSURANCEFEE" ]
		com.StaticTextIfDealedServiceFee.text				= Const[ "STATICTEXTIFDEALEDSERVICEFEE" ]
		com.StaticTextIfNDealedInsuranceFee.text		= Const[ "STATICTEXTIFNDEALEDINSURANCEFEE" ]
		com.StaticTextIfNDealedServiceFee.text			= Const[ "STATICTEXTIFNDEALEDSERVICEFEE" ]
		com.StaticTextIfDealedPictureFee.text				= Const[ "STATICTEXTPICTUREFEE" ]
		com.StaticTextIfNDealedPictureFee.text			= Const[ "STATICTEXTPICTUREFEE" ]
		com.StaticTextOtherFee.text									= Const[ "STATICTEXTOTHERFEE" ]
		com.StaticTextFrameFee.text									= Const[ "STATICTEXTFRAMEFEE" ]
		com.StaticTextFireFee.text									= Const[ "STATICTEXTFIREFEE" ]
		com.StaticTextIdentifyFee.text							= Const[ "STATICTEXTIDENTIFYFEE" ]
		com.StaticTextItemList.text									= Const[ "STATICTEXTITEMLIST" ]
		
		com.StaticTextPercent1.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent2.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent3.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent4.text									= Const[ "STATICTEXTPERCENT" ]
		
		com.StaticTextNTDperItem1.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem2.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem3.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem4.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem5.text							= Const[ "STATICTEXTNTDPERITEM" ]
		
		com.StaticTextItemName.text									= Const[ "STATICTEXTITEMNAME" ]
		com.StaticTextBuildImg.text									= Const[ "STATICTEXTBUILDIMG" ]
		com.StaticTextPackImg.text									= Const[ "STATICTEXTPACKIMG" ]
		
		com.StaticTextItemNum.text									= Const[ "STATICTEXTITEMNUM" ]
		com.StaticTextSpec.text											= Const[ "STATICTEXTSPEC" ]
		com.StaticTextReservePrice.text							= Const[ "STATICTEXTRESERVEPRICE" ]
		com.StaticTextReservePricePost.text					= Const[ "STATICTEXTRESERVEPRICEPOST" ]
		com.StaticTextRemain.text										= Const[ "STATICTEXTREMAIN" ]
		com.StaticTextItemPS.text										= Const[ "STATICTEXTITEMPS" ]
		com.StaticTextLotNO.text										= Const[ "STATICTEXTLOTNO" ]
		
		com.StaticTextPackImgState.text							= Const[ "STATICTEXTNOTSET" ]
		com.StaticTextBuildImgState.text						= Const[ "STATICTEXTNOTSET" ]
		
		com.StaticTextSerial.text										= ""
		
		# ���s
		com.ButtonAddDealer.label										= Const[ "BUTTONTEXTADD" ]
		com.ButtonFixDealer.label										= Const[ "BUTTONTEXTFIX" ]
		com.ButtonDelDealer.label										= Const[ "BUTTONTEXTDEL" ]
		com.ButtonAddItem.label											= Const[ "BUTTONTEXTADD" ]
		com.ButtonFixItem.label											= Const[ "BUTTONTEXTFIX" ]
		com.ButtonDelItem.label											= Const[ "BUTTONTEXTDEL" ]
		com.ButtonSetBuildImg.label									= Const[ "BUTTONTEXTSET" ]
		com.ButtonSetPackImg.label									= Const[ "BUTTONTEXTSET" ]
		
		# �Ϥ���������
		com.BitmapCanvasPack.visible = False
		com.BitmapCanvasBuild.visible = False
		
		# ���ճs�W��Ʈw
		while True:
			connect_ip = self.__connection_process()
			if connect_ip:
				break
			
		self.__add_msg( Const[ "ERRMSG_DB_CNCT_OK" ] % ( connect_ip, DB_CFG_PORT ), True )
		 
		# ���o��Ʈw������ƪ�
		self.__mongo_db = self.__mongo_client.bidding_data
		self.__file_store = gridfs.GridFS( self.__mongo_db )
		# print dir( self.__file_store )
		
		self.__dbtable_main					= self.__mongo_db.dealer_table
		self.__dbtable_item					= self.__mongo_db.dealer_item_table
		self.__dbtable_item_serial	= self.__mongo_db.dealer_item_serail
		self.__dbtable_auction			= self.__mongo_db.auctions_table
		
		# ������ܩҦ���a
		self.__gen_list_ui_by_data()
		
	# Ū���Ȧs��IP
	def __load_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "r" )
			
		except:
			return "127.0.0.1"
		
		line = file_ob.readline()
		if line[ -1 ] == "\n":
			line = line[ : -1 ]
		
		file_ob.close()
		return line
		
	# �x�s�Ȧs��IP
	def __save_cached_ip( self, connect_ip ):
		try:
			file_ob = open( CACHED_IP_PATH, "w" )
			
		except:
			return
		
		file_ob.write( connect_ip )
		file_ob.close()
	
	# ���ճs�W��Ʈw
	def __connection_process( self ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_AUCTION_IP" ], Const[ "READY_TO_CONNECT" ], self.__load_cached_ip() )
		if not result.accepted:
			return False
		
		connect_ip = result.text
		self.__save_cached_ip( connect_ip )
		
		try:
			self.__mongo_client = pymongo.MongoClient( connect_ip, DB_CFG_PORT )
		except:
			self.__add_msg( Const[ "ERRMSG_DB_CNCT_FAIL" ] % ( connect_ip, DB_CFG_PORT ), True )
			return False
			
		return connect_ip
	# ���շs�W�ϥΪ�
	def __add_dict( self, dict_data ):
		try:
			self.__dbtable_main.insert( dict_data )
		except:
			self.__add_msg( Const[ "ERRMSG_NAME_REPEAT" ], True )
			return 0
			
		return 1
		
	# ���o�֨����
	def __get_cache_data( self, type, sub_type ):
		return ATTR_MAP[ type ][ "CacheData" ][ sub_type ]
		
	# ���U �s�W�ϥΪ� ���s
	def on_ButtonAddDealer_mouseClick( self, event ):
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		dict_data = self.__gen_data_by_text_field()
		if not dict_data:
			return

		# ���J��ƨí��s��z����
		if not self.__add_dict( dict_data ):
			return
		
		self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "ADD_DEALER_SUCCESS" ] % self.__gen_dealer_title( dict_data ) )
		
		dealer_name = dict_data[ DEALER_KEY ]
		index, dict_data = self.__get_cache_data( "Dealer", "Main" )[ dealer_name ]
		self.components.ListDealer.selection = index
		
	# ��ܧǸ�
	def __show_serail( self, dict_data ):
		self.components.StaticTextSerial.text = "" if "_id" not in dict_data else Const[ "STATICTEXTSERIAL" ] % dict_data[ "_id" ]
	
	# ���U �s�W�D�� ���s
	def on_ButtonAddItem_mouseClick( self, event ):
		# �S�����������
		index, key = self.__get_key_by_selection( "Dealer" )
		if key not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		dict_data = self.__gen_item_data_by_text_field( key, None )
		if not dict_data:
			return

		dict_data[ "_id" ] = self.__gen_item_serial()
		
		# ���J��ƨí��s��z����
		if not self.__add_item_dict( dict_data ):
			return
		
		self.__gen_item_list_ui_by_data( dict_data[ "SrcDealer" ] )
		self.__add_msg( Const[ "ADD_ITEM_SUCCESS" ] % self.__gen_item_title( dict_data ) )
		
		item_key = dict_data[ ITEM_KEY ]
		index, dict_data = self.__get_cache_data( "Item", "Main" )[ item_key ]
		self.__show_serail( dict_data )
		self.components.ListItem.selection = index
		
	# ���U �s��R�a��� ���s
	def on_ButtonFixDealer_mouseClick( self, event ):
		# �S�����������
		index, key_ori = self.__get_key_by_selection( "Dealer" )
		if key_ori not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		dict_data = self.__gen_data_by_text_field()
		if not dict_data:
			return
		
		key = dict_data[ DEALER_KEY ]
		
		# �p�G�S��Key ���N�O��§���
		search_key = { DEALER_KEY: key_ori }
		if key_ori == key:
			self.__dbtable_main.update( search_key, dict_data )
		
		# �p�G�n��Key
		else:
			# ���մ��J��� �p�G���ѥN����
			if not self.__add_dict( dict_data ):
				return
			
			# �R���µP�����
			self.__dbtable_main.remove( search_key )
			
		self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "FIX_DATA_DONE" ] % self.__gen_dealer_title( dict_data ) )
		
		index, dict_data = self.__get_cache_data( "Dealer", "Main" )[ key ]
		self.components.ListDealer.selection = index
		
		
	# ���U �s���~��� ���s
	def on_ButtonFixItem_mouseClick( self, event ):
		# �S���������a
		index_src, key_src = self.__get_key_by_selection( "Dealer" )
		if key_src not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# �S�����������
		index, key = self.__get_key_by_selection( "Item" )
		if key not in self.__get_cache_data( "Item", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_ITEM" ] )
			return
		
		# ���o�Ϥ�ID �������i��n�R��
		index, dict_data_ori = self.__get_cache_data( "Item", "Main" )[ key ]
		
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		dict_data = self.__gen_item_data_by_text_field( key_src, key )
		if not dict_data:
			return
		
		# �w���o ��ƥi�R
		for pic_attr in ITEM_COM_DATA.iterkeys():
			self.__file_store.delete( dict_data_ori[ pic_attr ] )
			
		# �p�G�S��Key ���N�O��§���
		search_key = { ITEM_KEY: key }
		self.__dbtable_item.update( search_key, dict_data )
		
		self.__gen_item_list_ui_by_data( key_src )
		self.__add_msg( Const[ "FIX_DATA_DONE" ] % self.__gen_item_title( dict_data ) )
		
		index, dict_data = self.__get_cache_data( "Item", "Main" )[ key ]
		self.components.ListItem.selection = index
	
	# ���U �R����a��� ���s
	def on_ButtonDelDealer_mouseClick( self, event ):
		# �S�����������
		index, key = self.__get_key_by_selection( "Dealer" )
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		if key not in cache_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# ���o�n��ܪ���r/index
		index, dict_data = cache_data[ key ]
		mag_arg = self.__gen_dealer_title( dict_data );
		
		# �R���P�����
		self.__dbtable_main.remove( { DEALER_KEY: key } )
		self.__dbtable_item.remove( { "SrcDealer": key } )
		
		# ��s���
		index_the_last = self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "DEL_DATA_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListDealer.selection = index
		self.on_ListDealer_select()
		
	# ���U �R����~��� ���s
	def on_ButtonDelItem_mouseClick( self, event ):
		# �S���������a
		index_src, key_src = self.__get_key_by_selection( "Dealer" )
		if key_src not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# �S�����������
		index, key = self.__get_key_by_selection( "Item" )
		cache_data = self.__get_cache_data( "Item", "Main" )
		if key not in cache_data:
			self.__add_msg( Const[ "NO_SELECTED_ITEM" ] )
			return
		
		# ���o�n��ܪ���r/index
		index, dict_data = cache_data[ key ]
		mag_arg = self.__gen_item_title( dict_data );
		
		# ���R���Ϥ����
		for pic_attr in ITEM_COM_DATA.iterkeys():
			self.__file_store.delete( dict_data[ pic_attr ] )
		
		# �R����~���
		self.__dbtable_item.remove( { ITEM_KEY: key } )
		
		# ��s���
		index_the_last = self.__gen_item_list_ui_by_data( key_src )
		self.__add_msg( Const[ "DEL_DATA_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListItem.selection = index
		self.on_ListItem_select()
		
	# ���U�R�a�C����
	def on_ListDealer_select( self, event = None ):
		index, key = self.__get_key_by_selection( "Dealer" )
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		if key not in cache_data:
			return
		
		index, dict_data = cache_data[ key ]
		attr_list = ATTR_MAP[ "Dealer" ][ "ALL_ATTR" ]
		for attr in attr_list:
			value = dict_data[ attr ] if attr in dict_data else ""
			getattr( self.components, "TextField" + attr ).text = value
			
		self.__gen_item_list_ui_by_data( key )
		self.on_ListItem_select()
	
	# ���o�Ȧs�����ɦW
	def __get_temp_bmp_fname( self, keyword ):
		return "tmp_pic_%s" % keyword
		
	# ���U��~�C����
	def on_ListItem_select( self, event = None ):
		index, key = self.__get_key_by_selection( "Item" )
		cache_data = self.__get_cache_data( "Item", "Main" )
		if key not in cache_data:
			dict_data = {}
		else:
			index, dict_data = cache_data[ key ]
		
		# ��r����
		com = self.components
		attr_list = ATTR_MAP[ "Item" ][ "ALL_ATTR" ]
		for attr in attr_list:
			getattr( com, "TextField" + attr ).text = dict_data[ attr ] if attr in dict_data else ""
			
		# �Ϥ�����
		for pic_attr in ITEM_COM_DATA.iterkeys():
			pic_loaded = False
			if pic_attr in dict_data and dict_data[ pic_attr ]:
				fname = "tmp_pic_%s.jpg" % pic_attr
				file_obj = open( fname, "wb" )
				
				file_id = dict_data[ pic_attr ]
				file_id = ObjectId( file_id )
				if self.__file_store.exists( file_id ):
					# print type( file_id ), file_id
					file_obj_load = self.__file_store.get( file_id )
					file_obj.write( file_obj_load.read() )
					file_obj.close()
					pic_loaded = True
				else:
					self.__add_msg( Const[ "ERRMSG_IMG_ID_NOT_EXIST" ] % file_id )
			
			if not pic_loaded:
				fname = ""
			
			self.__set_image( pic_attr, fname, 0 )
			
		# �Ǹ�
		self.__show_serail( dict_data )
		
	# �Q�ί��ޭȨ��o�P��ID
	def __get_key_by_index( self, index, type ):
		cache_index = self.__get_cache_data( type, "Index" )
		if index in cache_index:
			return cache_index[ index ]
		
		return INVALID_KEY
		
	# �Q�Τ���������o�P��ID
	def __get_key_by_selection( self, type ):
		index = getattr( self.components, ATTR_MAP[ type ][ "ListCom" ] ).selection
		if index < 0:
			return index, INVALID_KEY
		
		return index, self.__get_key_by_index( index, type )
	
	# �Ʀr�P�w
	def __str_digit_test( self, chk_text ):
		try:
			float( chk_text )
			return_val = True
		except:
			# �Ŧr�����A�j�� �K�ߣt
			if chk_text == "":
				chk_text = "0"
				return_val = True
			else:
				return_val = False
		
		return return_val, chk_text
		
	# �Ѭɭ�����r��J�� ���ͽ�a���
	def __gen_data_by_text_field( self ):
		# �@�򥻸���˴�
		com = self.components
		
		dict_data = {}
		dealer_map = ATTR_MAP[ "Dealer" ]
		
		attr_list = dealer_map[ "ALL_ATTR" ]
		ne_attr_list = dealer_map[ "NONEMPTY_ATTR" ]
		dig_attr_list = dealer_map[ "DIGIT_ATTR" ]
		
		for attr in attr_list:
			text_com = getattr( com, "TextField" + attr )
			chk_text = text_com.text
			attr_name = getattr( com, "StaticText" + attr ).text
			
			# ������Ƥ��i����
			if attr in ne_attr_list and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % attr_name )
				return None
			
			# ������ƥ����O�Ʀr
			if attr in dig_attr_list:
				return_val, chk_text = self.__str_digit_test( chk_text )
				if return_val:
					text_com.text = chk_text
				else:
					self.__add_msg( Const[ "TEXT_CHK_FAIL_MUST_DIGIT" ] % attr_name )
					return None
			
			dict_data[ attr ] = chk_text
		
		# �إ�PK
		dict_data[ "_id" ] = dict_data[ "Name" ]
		return dict_data
		
	# �̶R�a��Ʋ��ͬɭ����
	def __gen_list_ui_by_data( self ):
		com = self.components
		
		com.ListDealer.clear()
		
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		cache_data.clear()
		cache_index = self.__get_cache_data( "Dealer", "Index" )
		cache_index.clear()
		
		index = -1
		for index, collection in enumerate( self.__dbtable_main.find().sort( DEALER_KEY, 1 ) ):
			dealer_name = collection[ DEALER_KEY ]
			
			cache_data[ dealer_name ] = index, collection
			cache_index[ index ] = dealer_name
			com.ListDealer.append( self.__gen_dealer_title( collection ) )
			
		return index
		
	# ���ͽ�a��ܦb�C���r��
	def __gen_dealer_title( self, dict_data ):
		return dict_data[ DEALER_KEY ]
		
	# ���͹D����ܦb�C���r��
	def __gen_item_title( self, dict_data ):
		return dict_data[ "ItemName" ]
		
	# �{�Ǭ���
	def __add_msg( self, msg, plus_pop = False ):
		if plus_pop:
			dialog.alertDialog( self, msg, Const[ "WARNING" ] )
		self.components.TextAreaLog.appendText( msg + "\n" )
		
	# ���ͽ�a��~�Ǹ�
	def __gen_item_serial( self ):
		# �إ�PK �q���X�Ǹ�
		year_now = time.localtime( time.time() )[ 0 ] - 1911
		serial = 0
		for collection in self.__dbtable_item_serial.find():
			year = collection[ "year" ]
			serial = collection[ "serial" ]
		
		# �p�G�O�Ū� ���J�@����� �_�h��s��
		table = self.__dbtable_item_serial
		if serial == 0:
			serial += 1
			table.insert( { "year": year_now, "serial": serial } )
		else:
			if year_now != year:
				serial = 0
			serial += 1
			table.update( { "year": year }, { "year": year_now, "serial": serial } )
		
		return "%d-%04d" % ( year_now, serial )
		
	# �Ѭɭ�����r��J�� ���ͽ�a��~���
	def __gen_item_data_by_text_field( self, dealer_name, item_key ):
		# �@�򥻸���˴�
		com = self.components
		
		# �S����ܽ�a
		index, key = self.__get_key_by_selection( "Dealer" )
		if key not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_DEALER_DATA" ] % key )
			return None
		
		dict_data = {}
		item_map = ATTR_MAP[ "Item" ]
		attr_list = item_map[ "ALL_ATTR" ]
		ne_attr_list = item_map[ "NONEMPTY_ATTR" ]
		dig_attr_list = item_map[ "DIGIT_ATTR" ]
		
		for attr in attr_list:
			text_com = getattr( com, "TextField" + attr )
			chk_text = text_com.text
			attr_name = getattr( com, "StaticText" + attr ).text
			
			# ������Ƥ��i����
			if attr in ne_attr_list and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % attr_name )
				return None
			
			# ������ƥ����O�Ʀr
			if attr in dig_attr_list:
				return_val, chk_text = self.__str_digit_test( chk_text )
				if return_val:
					text_com.text = chk_text
				else:
					self.__add_msg( Const[ "TEXT_CHK_FAIL_MUST_DIGIT" ] % attr_name )
					return None
			
			dict_data[ attr ] = chk_text
		
		# �ӷ���a
		dict_data[ "SrcDealer" ] = key
		
		# �ϥܸ��|
		for attr, com_name in ITEM_COM_DATA.iteritems():
			canvas = getattr( com, com_name )
			
			if hasattr( canvas, "path" ):
				try:
					file_obj = open( canvas.path, "rb" )
					file_id = self.__file_store.put( file_obj )
					file_id = str( file_id )
					# print file_id
				except:
					file_id = None
			else:
				file_id = None
			
			dict_data[ attr ] = file_id
		
		# �p�G�����~�s��
		lot_number = dict_data[ "LotNO" ]
		if lot_number != "":
			# �ˬd��~�s���O�_�s�b
			query_result = self.__dbtable_auction.find( { "AuctionId": lot_number } ).count()
			if not query_result:
				self.__add_msg( Const[ "TEXT_LOT_NO_NOT_EXIST" ] % lot_number )
				return None
				
			# �ˬd��~�s���ߤ@�� ����or�s�W�~�P�_
			if not item_key or lot_number != self.__dbtable_item.find( { "_id": item_key } )[ 0 ][ "LotNO" ]:
				query_result = self.__dbtable_item.find( { "LotNO": lot_number } ).count()
				if query_result:
					self.__add_msg( Const[ "TEXT_LOT_NO_REPEATED" ] % lot_number )
					return None

		return dict_data
		
	# ���շs�W�D��
	def __add_item_dict( self, dict_data ):
		try:
			self.__dbtable_item.insert( dict_data )
		except:
			self.__add_msg( Const[ "ERRMSG_NAME_REPEAT" ], True )
			return 0
			
		return 1
	
	# �̹D���Ʋ��ͬɭ����
	def __gen_item_list_ui_by_data( self, dealer_name ):
		com = self.components
		
		com.ListItem.clear()
		
		cache_data = self.__get_cache_data( "Item", "Main" )
		cache_data.clear()
		cache_index = self.__get_cache_data( "Item", "Index" )
		cache_index.clear()
		
		index = -1
		for index, collection in enumerate( self.__dbtable_item.find( { "SrcDealer": dealer_name } ).sort( ITEM_KEY, 1 ) ):
			item_key = collection[ ITEM_KEY ]
			
			cache_data[ item_key ] = index, collection
			cache_index[ index ] = item_key
			
			com.ListItem.append( self.__gen_item_title( collection ) )
			
		return index
		
	# �]�w�Ϥ����
	def __set_image( self, attr_name, path, need_msg ):
		com_name = ITEM_COM_DATA[ attr_name ]
		com = self.components
		canvas = getattr( com, com_name )
		
		canvas.visible = False
		
		try:
			canvas.visible = True
			canvas.clear()
			canvas.autoRefresh = 1
			
			bmp_obj = graphic.Bitmap( path )
			canvas.drawBitmapScaled( bmp_obj, ( 0, 0 ), canvas.size )
			
			canvas.path = path
		
		except:
			if need_msg:
				self.__add_msg( Const[ "ERRMSG_INVALID_IMG_FILE" ], 1 )
	
	# ���U �]�w�Ϥ� ���s
	def __set_image_path( self, attr_name ):
		result = dialog.openFileDialog( title = Const[ "OPEN_FILE_TITLE" ] )
		if not result.paths:
			return
		
		path = result.paths[ 0 ]
		self.__set_image( attr_name, path, 1 )
	
	def on_ButtonSetPackImg_mouseClick( self, event ):
		self.__set_image_path( "PackImg" )
	
	def on_ButtonSetBuildImg_mouseClick( self, event ):
		self.__set_image_path( "BuildImg" )
		
	# �k�䲾���Ϥ�
	def on_ButtonSetPackImg_mouseContextDown( self, event ):
		self.__on_del_image( "PackImg" )
		
	def on_ButtonSetBuildImg_mouseContextDown( self, event ):
		self.__on_del_image( "BuildImg" )

	def __on_del_image( self, attr_name ):
		result = dialog.singleChoiceDialog( self, "", Const[ "STATICTEXTDELIMAGETITLE" ], [ Const[ "STATICTEXTDEL" ], Const[ "STATICTEXTKEEP" ] ] )
		if result.selection == Const[ "STATICTEXTDEL" ]:
			self.__set_image( attr_name, "", 0 )
			
if __name__ == '__main__':
	app = model.Application(MyBackground)
	app.MainLoop()
