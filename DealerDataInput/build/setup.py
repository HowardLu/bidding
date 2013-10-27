#-*- coding: cp950 -*-

import sys

# import py2exe.mf as modulefinder
# for p in win32com.__path__[1:]:
	# modulefinder.AddPackagePath( "..\\", p)
# for extra in ["win32com.shell"]: #,"win32com.mapi"
	# __import__(extra)
	# m = sys.modules[extra]
	# for p in m.__path__[1:]:
			# modulefinder.AddPackagePath(extra, p)

from distutils.core import setup
from general import run_main_process

# py2exe stuff
import py2exe, os

SYS_ARG = sys.argv[ 1 ]

def main():
	# includes for py2exe
	# �O�����{���N��win ���M�N��pure
	comp_list = [ "button", "image", "staticbox", "statictext", "textarea", "textfield", "passwordfield", "choice", "list", "bitmapcanvas" ]
	includes = []
	for comp in comp_list:
		includes += [ "PythonCard.components." + comp ]

	opts	= { "py2exe": { "includes": includes } }
	# opts	= { "py2exe": { "includes": [] } }
	
	# end of py2exe stuff
	pyfname = "..\src\DealerDataInput"
	fname = pyfname.split( "\\" )[ -1 ]
	res_path = pyfname[ : -( len( fname ) ) ]
	pyfname += ".py"
	
	# �����{���|���귽�� ��ݬݦ��S��
	pycard_resources = []
	for filename in os.listdir( res_path ):
		if filename.find( ".rsrc." ) > -1:
			pycard_resources += [ res_path + filename ]
	
	# �@��module
	setup(	name = fname,
					package_dir = { fname: "." },
					packages = [ fname ],
					data_files = [ ( ".", pycard_resources ) ],
					console = [ pyfname ],
					# windows = [ { "script": pyfname } ],
					options = opts
				)

if __name__ == "__main__":
	if SYS_ARG == "install":
		main()
	else:
		run_main_process( main )
