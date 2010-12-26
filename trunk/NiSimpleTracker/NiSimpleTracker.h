// 以下の ifdef ブロックは DLL からのエクスポートを容易にするマクロを作成するための 
// 一般的な方法です。この DLL 内のすべてのファイルは、コマンド ラインで定義された NISIMPLETRACKER_EXPORTS
// シンボルでコンパイルされます。このシンボルは、この DLL を使うプロジェクトで定義することはできません。
// ソースファイルがこのファイルを含んでいる他のプロジェクトは、 
// NISIMPLETRACKER_API 関数を DLL からインポートされたと見なすのに対し、この DLL は、このマクロで定義された
// シンボルをエクスポートされたと見なします。
#ifdef NISIMPLETRACKER_EXPORTS
#define NISIMPLETRACKER_API __declspec(dllexport)
#else
#define NISIMPLETRACKER_API __declspec(dllimport)
#endif

// このクラスは NiSimpleTracker.dll からエクスポートされました。
class NISIMPLETRACKER_API CNiSimpleTracker {
public:
	CNiSimpleTracker(void);
	// TODO: メソッドをここに追加してください。
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
