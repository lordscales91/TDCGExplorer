// �ȉ��� ifdef �u���b�N�� DLL ����̃G�N�X�|�[�g��e�Ղɂ���}�N�����쐬���邽�߂� 
// ��ʓI�ȕ��@�ł��B���� DLL ���̂��ׂẴt�@�C���́A�R�}���h ���C���Œ�`���ꂽ NISIMPLETRACKER_EXPORTS
// �V���{���ŃR���p�C������܂��B���̃V���{���́A���� DLL ���g���v���W�F�N�g�Œ�`���邱�Ƃ͂ł��܂���B
// �\�[�X�t�@�C�������̃t�@�C�����܂�ł��鑼�̃v���W�F�N�g�́A 
// NISIMPLETRACKER_API �֐��� DLL ����C���|�[�g���ꂽ�ƌ��Ȃ��̂ɑ΂��A���� DLL �́A���̃}�N���Œ�`���ꂽ
// �V���{�����G�N�X�|�[�g���ꂽ�ƌ��Ȃ��܂��B
#ifdef NISIMPLETRACKER_EXPORTS
#define NISIMPLETRACKER_API __declspec(dllexport)
#else
#define NISIMPLETRACKER_API __declspec(dllimport)
#endif

// ���̃N���X�� NiSimpleTracker.dll ����G�N�X�|�[�g����܂����B
class NISIMPLETRACKER_API CNiSimpleTracker {
public:
	CNiSimpleTracker(void);
	// TODO: ���\�b�h�������ɒǉ����Ă��������B
};

extern "C" {
extern NISIMPLETRACKER_API int nNiSimpleTracker;

NISIMPLETRACKER_API int fnNiSimpleTracker(void);
NISIMPLETRACKER_API int OpenNIClean(void);
NISIMPLETRACKER_API XnUInt16 OpenNIGetXRes(void);
NISIMPLETRACKER_API XnUInt16 OpenNIGetYRes(void);
NISIMPLETRACKER_API unsigned char* OpenNIGetDepthBuf(void);
NISIMPLETRACKER_API XnSkeletonJointPosition* OpenNIGetJointPos(void);
NISIMPLETRACKER_API int OpenNIInit(char* path);
NISIMPLETRACKER_API void OpenNIDrawDepthMap(void);

}
