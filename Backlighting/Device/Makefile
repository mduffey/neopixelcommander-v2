
OS = WINDOWS

PROG = rawhid

# To set up Ubuntu Linux to cross compile for Windows:
#
#    apt-get install mingw32 mingw32-binutils mingw32-runtime
#
# Just edit the variable above for WINDOWS, then use "make" to build rawhid

# To import this dll into an MSVC project you need an associated .lib file.
# To create the .lib file, open a Visual Studio developer command prompt as administrator
# and type the following command:
# 
# lib /machine:i386 /def:<path_to_here>\hiddll.def



TARGET = $(PROG).dll
#CC = i586-mingw32msvc-gcc
CC = gcc
CFLAGS = -Wall -O2 -DOS_$(OS) -DBUILDING_EXAMPLE_DLL 
LIBS = -lhid -lsetupapi
OBJS = hid.o


all: $(TARGET)

$(TARGET): $(OBJS)
	$(CC) -shared -o $(TARGET) $(OBJS) -Wl,--output-def,$(PROG).def,--out-implib,lib$(PROG).a $(LIBS)

hid.o: hid_$(OS).c hid.h
	$(CC) $(CFLAGS) -c -o $@ $<

clean:
	rm -f *.o $(PROG) $(PROG).dmg $(PROG).dll $(PROG).def lib$(PROG).a



