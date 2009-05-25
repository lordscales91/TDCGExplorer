CFLAGS += -Wall
CLINKFLAGS += -framework Cg -framework OpenGL -framework GLUT -framework Foundation
all: tsoview tsodump matline tmodump

tsoview: tsoview.o matrix.o tdcg.o tsofile.o texture.o param.o effect.o material.o mesh.o tmofile.o frame.o
	$(CC) -o tsoview tsoview.o matrix.o tdcg.o tsofile.o texture.o param.o effect.o material.o mesh.o tmofile.o frame.o $(CLINKFLAGS)

tsodump: tsodump.o matrix.o tdcg.o tsofile.o texture.o effect.o material.o mesh.o
	$(CC) -o tsodump tsodump.o matrix.o tdcg.o tsofile.o texture.o effect.o material.o mesh.o

matline: matline.o matrix.o tdcg.o tsofile.o texture.o effect.o param.o material.o mesh.o
	$(CC) -o matline matline.o matrix.o tdcg.o tsofile.o texture.o effect.o param.o material.o mesh.o

tmodump: tmodump.o matrix.o tdcg.o tmofile.o frame.o
	$(CC) -o tmodump tmodump.o matrix.o tdcg.o tmofile.o frame.o

clean:
	$(RM) *.o
