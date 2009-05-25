/* program.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>    /* for printf and NULL */
#include <stdlib.h>   /* for exit */
#include <string.h>
#include <math.h>     /* for sqrt, sin, and cos */
#include <assert.h>   /* for assert */
#include <GLUT/glut.h>
#include <Cg/cg.h>    /* Can't include this?  Is Cg Toolkit installed! */
#include <Cg/cgGL.h>

#include "matrix.h"
#include "tdcg.h"
#include "tsofile.h"
#include "tmofile.h"

static CGcontext   myCgContext;
static CGprofile   myCgVertexProfile,
                   myCgFragmentProfile;
static CGprogram   myCgVertexProgram,
                   myCgFragmentProgram;
static CGparameter myCgVertexParam_modelViewProj;
static CGparameter myCgVertexParam_localBoneMats;
static CGparameter myCgFragmentParam_shadeTex;                   
static CGparameter myCgFragmentParam_colorTex;                   

static const char *myProgramName = "tsoview",
                  *myVertexProgramFileName = "C4E1v_transform.cg",
/* Page 97 */     *myVertexProgramName = "C4E1v_transform",
                  *myFragmentProgramFileName = "C3E3f_texture.cg",
/* Page 67 */     *myFragmentProgramName = "C3E3f_texture";

static float myEyeAngle = 0.0;   /* Angle eye rotates around scene. */
static float myProjectionMatrix[16];

static void checkForCgError(const char *situation)
{
  CGerror error;
  const char *string = cgGetLastErrorString(&error);

  if (error != CG_NO_ERROR) {
    printf("%s: %s: %s\n",
      myProgramName, situation, string);
    if (error == CG_COMPILER_ERROR) {
      printf("%s\n", cgGetLastListing(myCgContext));
    }
    exit(1);
  }
}

#define BUFFER_OFFSET(i) ((char *)NULL + (i))

Tsofile *tso;
Tmofile *tmo;

void node_make_combined_matrix_with_tmo(Node *node, Matrix m)
{
    Matrix transform;
    int tmo_node_idx = tmo_find_node_idx(tmo, node->name);
    assert(tmo_node_idx < tmo->nnodes);
    assert(tmo_node_idx < tmo->frames[0]->nmatrices);
    if (tmo_node_idx != -1)
	transform = tmo->frames[0]->matrices[tmo_node_idx];
    else
	transform = node->transform;
    
    multMatrix(node->combined.m, m.m, transform.m);

    Node *bone = node->children_head;
    while (bone != NULL)
    {
	node_make_combined_matrix_with_tmo(bone, node->combined);
	bone = bone->children_next;
    }
}

void make_combined_matrix_with_tmo()
{
    Matrix m;
    makeIdentityMatrix(m.m);

    Node *root = tso->nodes[0];
    node_make_combined_matrix_with_tmo(root, m);
}

int find_texture_idx(char *name)
{
    int k;
    for (k=0; k<tso->ntextures; k++)
    {
	Texture *tex = tso->textures[k];
	if (!strcmp(tex->name, name))
	    return k+32;
    }
    return 0;
}

void cube()
{
    int i;
    for (i=0; i<tso->nmeshes; i++)
    {
	Mesh *mesh = tso->meshes[i];

	int j;
	for (j=0; j<mesh->nsubs; j++)
	{
	    Submesh *sub = mesh->subs[j];

	    float bone_matrices[16*16];
	    /* assign local bone matrices */
	    int k;
	    for (k=0; k<sub->nbones; k++)
	    {
		Node *node = tso->nodes[sub->bones[k]];
		//printf("bone %s ", node->short_name);

		Matrix m;
		multMatrix(m.m, node->combined.m, node->offset.m);

		memcpy(bone_matrices+k*16, m.m, sizeof(float)*16);
		//printMatrix("bm", bone_matrices+k*16);
	    }
	    cgGLSetMatrixParameterArrayfr(myCgVertexParam_localBoneMats, 0, sub->nbones, bone_matrices);

	    Material *material = tso->materials[sub->spec];
	    GLuint shadeTex_idx = 0;
	    GLuint colorTex_idx = 0;

	    for (k=0; k<material->nlines; k++)
	    {
		char *line = material->lines[k];
		Param *param = create_param();
		param_read(param, line);
		if (!strcmp(param->type, "texture"))
		{
		    if (!strcmp(param->name, "ShadeTex"))
			shadeTex_idx = (GLuint)find_texture_idx(param->value);
		    else if (!strcmp(param->name, "ColorTex"))
			colorTex_idx = (GLuint)find_texture_idx(param->value);
		}
		free_param(param);
	    }
	    assert(shadeTex_idx != 0);
	    assert(colorTex_idx != 0);

	    cgGLSetTextureParameter(myCgFragmentParam_shadeTex, shadeTex_idx);
	    checkForCgError("setting shadeTex 2D texture");

	    cgGLSetTextureParameter(myCgFragmentParam_colorTex, colorTex_idx);
	    checkForCgError("setting colorTex 2D texture");

	    cgGLEnableTextureParameter(myCgFragmentParam_shadeTex);
	    checkForCgError("enable shadeTex texture");

	    cgGLEnableTextureParameter(myCgFragmentParam_colorTex);
	    checkForCgError("enable colorTex texture");

	    cgUpdateProgramParameters(myCgVertexProgram);
	    cgUpdateProgramParameters(myCgFragmentProgram);

	    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh->bufObje[j]);
	    glBindBuffer(GL_ARRAY_BUFFER, mesh->bufObjs[j]);

	    /* position */
	    glVertexPointer(3, GL_FLOAT, sizeof(Vertex), BUFFER_OFFSET(0));
	    glEnableClientState(GL_VERTEX_ARRAY);

	    /* normal */
	    glNormalPointer(GL_FLOAT, sizeof(Vertex), BUFFER_OFFSET(7*sizeof(float)+sizeof(int)*4));
	    glEnableClientState(GL_NORMAL_ARRAY);

	    /* uv */
	    glClientActiveTexture(GL_TEXTURE0);
	    glTexCoordPointer(2, GL_FLOAT, sizeof(Vertex), BUFFER_OFFSET(10*sizeof(float)+sizeof(int)*4));
	    glEnableClientState(GL_TEXTURE_COORD_ARRAY);

	    /* boneWgts */
	    glClientActiveTexture(GL_TEXTURE3);
	    glTexCoordPointer(4, GL_FLOAT, sizeof(Vertex), BUFFER_OFFSET(3*sizeof(float)));
	    glEnableClientState(GL_TEXTURE_COORD_ARRAY);

	    /* boneIdxs */
	    glClientActiveTexture(GL_TEXTURE4);
	    glTexCoordPointer(4, GL_INT, sizeof(Vertex), BUFFER_OFFSET(7*sizeof(float)));
	    glEnableClientState(GL_TEXTURE_COORD_ARRAY);

	    glDrawElements(GL_TRIANGLE_STRIP, sub->nindices, GL_UNSIGNED_SHORT, BUFFER_OFFSET(0));

	    cgGLDisableTextureParameter(myCgFragmentParam_shadeTex);
	    checkForCgError("disabling shadeTex texture");

	    cgGLDisableTextureParameter(myCgFragmentParam_colorTex);
	    checkForCgError("disabling colorTex texture");
	}
    }
}

void idle()
{
    glutPostRedisplay();
}

void display(void)
{
  float modelMatrix[16], viewMatrix[16],
        modelViewMatrix[16], modelViewProjMatrix[16];

  makeLookAtMatrix(40*sin(myEyeAngle), 10, 40*cos(myEyeAngle),  /* eye position */
                    0, 10, 0, /* view center */
                    0, 1, 0, /* up vector */
                    viewMatrix);

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

  cgGLBindProgram(myCgVertexProgram);
  checkForCgError("binding vertex program");

  cgGLEnableProfile(myCgVertexProfile);
  checkForCgError("enabling vertex profile");

  cgGLBindProgram(myCgFragmentProgram);
  checkForCgError("binding fragment program");

  cgGLEnableProfile(myCgFragmentProfile);
  checkForCgError("enabling fragment profile");

  /*** Render green wireframe sphere ***/

  /* modelView = rotateMatrix * translateMatrix */
  makeIdentityMatrix(modelMatrix);

  /* modelViewMatrix = viewMatrix * modelMatrix */
  multMatrix(modelViewMatrix, viewMatrix, modelMatrix);

  /* modelViewProj = projectionMatrix * modelViewMatrix */
  multMatrix(modelViewProjMatrix, myProjectionMatrix, modelViewMatrix);

  /* Set matrix parameter with row-major matrix. */
  cgGLSetMatrixParameterfr(myCgVertexParam_modelViewProj, modelViewProjMatrix);

    cube();

  cgGLDisableProfile(myCgVertexProfile);
  checkForCgError("disabling vertex profile");

  cgGLDisableProfile(myCgFragmentProfile);
  checkForCgError("disabling fragment profile");

    glutSwapBuffers();
}

void reshape(int width, int height)
{
  double aspectRatio = (float) width / (float) height;
  double fieldOfView = 30.0; /* Degrees */

  /* Build projection matrix once. */
  makePerspectiveMatrix(fieldOfView, aspectRatio,
                         1.0, 1000.0,  /* Znear and Zfar */
                         myProjectionMatrix);
  glViewport(0, 0, width, height);
}

void mouse(int button, int state, int x, int y)
{
    switch (button)
    {
    case GLUT_LEFT_BUTTON:
	if (state == GLUT_DOWN)
	    glutIdleFunc(idle);
	else
	    glutIdleFunc(0);
	break;
    }
}

void mesh_delete_buffers(Mesh * mesh)
{
    int nbufs = mesh->nsubs;
    glDeleteBuffers(nbufs, mesh->bufObje);
    glDeleteBuffers(nbufs, mesh->bufObjs);
}

void keyboard(unsigned char key, int x, int y)
{
    switch (key)
    {
	case '\033':
	{
	    int i;
	    for (i=0; i<tso->nmeshes; i++)
	    {
		Mesh *mesh = tso->meshes[i];

		mesh_delete_buffers(mesh);
	    }
	}
	    cgDestroyProgram(myCgFragmentProgram);
	    cgDestroyProgram(myCgVertexProgram);
	    cgDestroyContext(myCgContext);
	    free_tmo(tmo);
	    tmo = NULL;
	    free_tso(tso);
	    tso = NULL;
	    exit(0);
	default:
	    break;
    }
}

void tso_bind_textures(Tsofile *tso)
{
    int i;
    for (i=0; i<tso->ntextures; i++)
    {
	glBindTexture(GL_TEXTURE_2D, i+32);
	Texture *tex = tso->textures[i];
	assert(tex->depth == 4);
	/* Load texture with mipmaps. */
	gluBuild2DMipmaps(GL_TEXTURE_2D, GL_RGBA, tex->width, tex->height, GL_RGBA, GL_UNSIGNED_INT_8_8_8_8_REV, tex->data);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
    }
}

void mesh_gen_buffers(Mesh *mesh)
{
    int nbufs = mesh->nsubs;
    mesh->bufObjs = (GLuint *)malloc(sizeof(GLuint)*nbufs);
    mesh->bufObje = (GLuint *)malloc(sizeof(GLuint)*nbufs);
    glGenBuffers(nbufs, mesh->bufObjs);
    glGenBuffers(nbufs, mesh->bufObje);
}

void mesh_gen_indices(Mesh *mesh)
{
    int j;
    for (j=0; j<mesh->nsubs; j++)
    {
	Submesh *sub = mesh->subs[j];

	sub->nindices = sub->nverts;
	sub->indices = (GLushort *)malloc(sizeof(GLushort)*sub->nindices);
	{
	    GLushort *x = sub->indices;
	    int k;
	    for (k=0; k<sub->nindices; k++, x++)
		*x = (GLushort)k;
	}
    }
}

void mesh_bind_buffers(Mesh *mesh)
{
    int j;
    for (j=0; j<mesh->nsubs; j++)
    {
	Submesh *sub = mesh->subs[j];

	glBindBuffer(GL_ARRAY_BUFFER, mesh->bufObjs[j]);
	glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex)*sub->nverts, sub->vertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh->bufObje[j]);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(GLushort)*sub->nindices, sub->indices, GL_STATIC_DRAW);
    }
}

void init()
{
    /* tso_dump(tso); */
    make_combined_matrix_with_tmo();

    glClearColor(0.1, 0.3, 0.6, 0.0);
    glEnable(GL_DEPTH_TEST);
    glEnable(GL_CULL_FACE);
    glCullFace(GL_BACK);

    glEnable(GL_MULTISAMPLE);
    glEnable(GL_SAMPLE_ALPHA_TO_COVERAGE);

    glPixelStorei(GL_UNPACK_ALIGNMENT, 1); /* Tightly packed texture data. */
    tso_bind_textures(tso);

  myCgContext = cgCreateContext();
  checkForCgError("creating context");
  cgGLSetDebugMode(CG_FALSE);
  cgSetParameterSettingMode(myCgContext, CG_DEFERRED_PARAMETER_SETTING);

  myCgVertexProfile = cgGLGetLatestProfile(CG_GL_VERTEX);
  cgGLSetOptimalOptions(myCgVertexProfile);
  checkForCgError("selecting vertex profile");

  myCgVertexProgram =
    cgCreateProgramFromFile(
      myCgContext,              /* Cg runtime context */
      CG_SOURCE,                /* Program in human-readable form */
      myVertexProgramFileName,  /* Name of file containing program */
      myCgVertexProfile,        /* Profile: OpenGL ARB vertex program */
      myVertexProgramName,      /* Entry function name */
      NULL);                    /* No extra compiler options */
  checkForCgError("creating vertex program from file");
  cgGLLoadProgram(myCgVertexProgram);
  checkForCgError("loading vertex program");

  myCgVertexParam_modelViewProj = cgGetNamedParameter(myCgVertexProgram, "wvp");
  checkForCgError("could not get modelViewProj parameter");

  myCgVertexParam_localBoneMats = cgGetNamedParameter(myCgVertexProgram, "localBoneMats");
  checkForCgError("could not get localBoneMats parameter");

  myCgFragmentProfile = cgGLGetLatestProfile(CG_GL_FRAGMENT);
  cgGLSetOptimalOptions(myCgFragmentProfile);
  checkForCgError("selecting fragment profile");

  myCgFragmentProgram =
    cgCreateProgramFromFile(
      myCgContext,                /* Cg runtime context */
      CG_SOURCE,                  /* Program in human-readable form */
      myFragmentProgramFileName,  /* Name of file containing program */
      myCgFragmentProfile,        /* Profile: OpenGL ARB vertex program */
      myFragmentProgramName,      /* Entry function name */
      NULL);                      /* No extra compiler options */
  checkForCgError("creating fragment program from file");
  cgGLLoadProgram(myCgFragmentProgram);
  checkForCgError("loading fragment program");

  myCgFragmentParam_shadeTex = cgGetNamedParameter(myCgFragmentProgram, "shadeTex");
  checkForCgError("getting shadeTex parameter");

  myCgFragmentParam_colorTex = cgGetNamedParameter(myCgFragmentProgram, "colorTex");
  checkForCgError("getting colorTex parameter");

    int i;
    for (i=0; i<tso->nmeshes; i++)
    {
	Mesh *mesh = tso->meshes[i];

	mesh_gen_buffers(mesh);
	mesh_gen_indices(mesh);
	mesh_bind_buffers(mesh);
    }
}

int main(int argc, char* argv[])
{
    FILE* tso_file;
    FILE* tmo_file;

    if (argc != 3)
    {
	printf("Usage: tsoview <tso file> <tmo file>\n");
	return 1;
    }
    char* tso_filename = argv[1];
    puts(tso_filename);
    char* tmo_filename = argv[2];
    puts(tmo_filename);

    tso = create_tso();
    tmo = create_tmo();

    tso_file = fopen(tso_filename, "rb");
    if (!tso_file)
    {
	free_tmo(tmo);
	free_tso(tso);
	return 1;
    }
    int magic;
    magic = read_int(tso_file);
    printf("magic %08x\n", magic);
    if (magic != (int)(('T'<<8*0)+('S'<<8*1)+('O'<<8*2)+('1'<<8*3)))
    {
	fclose(tso_file);
	free_tmo(tmo);
	free_tso(tso);
	return 1;
    }

    tso_read(tso, tso_file);
    fclose(tso_file);

    tmo_file = fopen(tmo_filename, "rb");
    if (!tmo_file)
    {
	free_tmo(tmo);
	free_tso(tso);
	return 1;
    }

    magic = read_int(tmo_file);
    printf("magic %08x\n", magic);
    if (magic != (int)(('T'<<8*0)+('M'<<8*1)+('O'<<8*2)+('1'<<8*3)))
    {
	fclose(tmo_file);
	free_tmo(tmo);
	free_tso(tso);
	return 1;
    }

    tmo_read(tmo, tmo_file);
    fclose(tmo_file);

    glutInitWindowSize(800, 600);
    glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE | GLUT_DEPTH | GLUT_MULTISAMPLE);
    glutInit(&argc, argv);

    glutCreateWindow(myProgramName);
    printf("version %s\n", glGetString(GL_VERSION));

    GLint buffers, samples;
    glGetIntegerv(GL_SAMPLE_BUFFERS, &buffers);
    glGetIntegerv(GL_SAMPLES, &samples);
    printf("buffers %d samples %d\n", buffers, samples);

    glutDisplayFunc(display);
    glutReshapeFunc(reshape);
    glutMouseFunc(mouse);
    glutKeyboardFunc(keyboard);

    init();
    glutMainLoop();

    return 0;
}
