/* frame.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    int nmatrices;
    Matrix *matrices;
} Frame;

Frame *create_frame();
void frame_read(Frame *frame, FILE *file);
void frame_dump(Frame *frame);
void free_frame(Frame *frame);
