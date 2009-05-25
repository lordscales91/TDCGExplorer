/* tdcg.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    float m[16];
} Matrix;

typedef struct {
    float position[3];
    float weights[4];
    int indices[4];
    float normal[3];
    float u;
    float v;
    //int nweights;
} Vertex;

int read_int(FILE* file);
float read_float(FILE* file);
void read_matrix(Matrix* ret, FILE* file);
void read_vertex(Vertex* ret, FILE* file);
void read_string(char* ret, FILE* file);
