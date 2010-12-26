// NiSimpleTracker.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "NiSimpleTracker.h"

// include libraries
#pragma comment(lib, "openNI.lib")

// callbacks
void XN_CALLBACK_TYPE User_NewUser(xn::UserGenerator&,XnUserID,void*);
void XN_CALLBACK_TYPE User_LostUser(xn::UserGenerator&,XnUserID,void*);
void XN_CALLBACK_TYPE UserCalibration_CalibrationStart(xn::SkeletonCapability&,XnUserID,void*);
void XN_CALLBACK_TYPE UserCalibration_CalibrationEnd(xn::SkeletonCapability&,XnUserID,XnBool,void*);
void XN_CALLBACK_TYPE UserPose_PoseDetected(xn::PoseDetectionCapability&,const XnChar*,XnUserID,void*);

xn::Context				g_Context;
xn::DepthGenerator		g_DepthGenerator;
xn::UserGenerator		g_UserGenerator;
XnBool					g_bNeedPose = FALSE;
XnChar					g_strPose[20] = "";

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		OpenNIClean();
		break;
	}
	return TRUE;
}

// CALLBACK:User_NewUser()
void XN_CALLBACK_TYPE User_NewUser(xn::UserGenerator& generator, XnUserID nId, void* pCookie)
{
	printf("New User %d\n", nId);
	// New user found
	if (g_bNeedPose)
	{
		g_UserGenerator.GetPoseDetectionCap().StartPoseDetection(g_strPose, nId);
	}
	else
	{
		g_UserGenerator.GetSkeletonCap().RequestCalibration(nId, TRUE);
	}
}

// CALLBACK:User_LostUser()
void XN_CALLBACK_TYPE User_LostUser(xn::UserGenerator& generator, XnUserID nId, void* pCookie)
{
	printf("Lost user %d\n", nId);
}

// CALLBACK:UserCalibration_CalibrationStart()
void XN_CALLBACK_TYPE UserCalibration_CalibrationStart(xn::SkeletonCapability& capability, XnUserID nId, void* pCookie)
{
	printf("Calibration started for user %d\n", nId);
}

// CALLBACK:UserCalibration_CalibrationEnd()
void XN_CALLBACK_TYPE UserCalibration_CalibrationEnd(xn::SkeletonCapability& capability, XnUserID nId, XnBool bSuccess, void* pCookie)
{
	if (bSuccess)
	{
		// Calibration succeeded
		printf("Calibration complete, start tracking user %d\n", nId);
		g_UserGenerator.GetSkeletonCap().StartTracking(nId);
	}
	else
	{
		// Calibration failed
		printf("Calibration failed for user %d\n", nId);
		if (g_bNeedPose)
		{
			g_UserGenerator.GetPoseDetectionCap().StartPoseDetection(g_strPose, nId);
		}
		else
		{
			g_UserGenerator.GetSkeletonCap().RequestCalibration(nId, TRUE);
		}
	}
}

// CALLBACK:UserPose_PoseDetected()
void XN_CALLBACK_TYPE UserPose_PoseDetected(xn::PoseDetectionCapability& capability, const XnChar* strPose, XnUserID nId, void* pCookie)
{
	printf("Pose %s detected for user %d\n", strPose, nId);
	g_UserGenerator.GetPoseDetectionCap().StopPoseDetection(nId);
	g_UserGenerator.GetSkeletonCap().RequestCalibration(nId, TRUE);
}

extern "C" {
// これは、エクスポートされた変数の例です。
NISIMPLETRACKER_API int nNiSimpleTracker=0;

// これは、エクスポートされた関数の例です。
NISIMPLETRACKER_API int fnNiSimpleTracker(void)
{
	return 42;
}
#define CHECK_RC(nRetVal, what)										\
	if (nRetVal != XN_STATUS_OK)									\
	{																\
		printf("%s failed: %s\n", what, xnGetStatusString(nRetVal));\
		return nRetVal;												\
	}

XnBool g_bDrawBackground = TRUE;
XnBool g_bDrawPixels = TRUE;

XnBool g_bPause = false;

#define MAX_DEPTH 10000
float g_pDepthHist[MAX_DEPTH];
unsigned int getClosestPowerOfTwo(unsigned int n)
{
	unsigned int m = 2;
	while(m < n) m<<=1;

	return m;
}

static unsigned char* pDepthBuf;
static XnSkeletonJointPosition* pJointPos;
static XnBool g_bTracking = false;

NISIMPLETRACKER_API int OpenNIInit(char* path)
{
	XnStatus nRetVal = XN_STATUS_OK;

	nRetVal = g_Context.InitFromXmlFile(path);
	CHECK_RC(nRetVal, "InitFromXml");

	nRetVal = g_Context.FindExistingNode(XN_NODE_TYPE_DEPTH, g_DepthGenerator);
	CHECK_RC(nRetVal, "Find depth generator");
	nRetVal = g_Context.FindExistingNode(XN_NODE_TYPE_USER, g_UserGenerator);
	if (nRetVal != XN_STATUS_OK)
	{
		nRetVal = g_UserGenerator.Create(g_Context);
		CHECK_RC(nRetVal, "Find user generator");
	}

	XnCallbackHandle hUserCallbacks, hCalibrationCallbacks, hPoseCallbacks;
	if (!g_UserGenerator.IsCapabilitySupported(XN_CAPABILITY_SKELETON))
	{
		printf("Supplied user generator doesn't support skeleton\n");
		return 1;
	}
	g_UserGenerator.RegisterUserCallbacks(User_NewUser, User_LostUser, NULL, hUserCallbacks);
	g_UserGenerator.GetSkeletonCap().RegisterCalibrationCallbacks(UserCalibration_CalibrationStart, UserCalibration_CalibrationEnd, NULL, hCalibrationCallbacks);

	if (g_UserGenerator.GetSkeletonCap().NeedPoseForCalibration())
	{
		g_bNeedPose = TRUE;
		if (!g_UserGenerator.IsCapabilitySupported(XN_CAPABILITY_POSE_DETECTION))
		{
			printf("Pose required, but not supported\n");
			return 1;
		}
		g_UserGenerator.GetPoseDetectionCap().RegisterToPoseCallbacks(UserPose_PoseDetected, NULL, NULL, hPoseCallbacks);
		g_UserGenerator.GetSkeletonCap().GetCalibrationPose(g_strPose);
	}

	g_UserGenerator.GetSkeletonCap().SetSkeletonProfile(XN_SKEL_PROFILE_ALL);

	nRetVal = g_Context.StartGeneratingAll();
	CHECK_RC(nRetVal, "StartGenerating");

	xn::SceneMetaData smd;
	xn::DepthMetaData dmd;
	g_DepthGenerator.GetMetaData(dmd);

	if (!g_bPause)
	{
		// Read next available data
		g_Context.WaitAndUpdateAll();
	}

	// Process the data
	g_DepthGenerator.GetMetaData(dmd);
	g_UserGenerator.GetUserPixels(0, smd);

	static bool bInitialized = false;	

	if(!bInitialized)
	{
		bInitialized = true;

		XnUInt16 g_nXRes = dmd.XRes();
		XnUInt16 g_nYRes = dmd.YRes();
		pDepthBuf = new unsigned char[g_nXRes * g_nYRes * 4];
		pJointPos = new XnSkeletonJointPosition[15];
	}
	return 0;
}

XnFloat Colors[][3] =
{
	{0,1,1},
	{0,0,1},
	{0,1,0},
	{1,1,0},
	{1,0,0},
	{1,.5,0},
	{.5,1,0},
	{0,.5,1},
	{.5,0,1},
	{1,1,.5},
	{1,1,1}
};
XnUInt32 nColors = 10;

NISIMPLETRACKER_API int OpenNIClean(void)
{
	printf("Shutdown now.\n");
	if (pJointPos)
	{
		delete pJointPos;
                pJointPos = NULL;
	}
	if (pDepthBuf)
	{
		delete pDepthBuf;
                pDepthBuf = NULL;
	}
	g_Context.Shutdown();
	return 42;
}
NISIMPLETRACKER_API unsigned char* OpenNIGetDepthBuf(void)
{
	return pDepthBuf;
}
NISIMPLETRACKER_API XnSkeletonJointPosition* OpenNIGetJointPos(void)
{
	return pJointPos;
}
NISIMPLETRACKER_API XnBool OpenNIIsTracking(void)
{
	return g_bTracking;
}
NISIMPLETRACKER_API void OpenNIDrawDepthMap(void)
{
	xn::SceneMetaData smd;
	xn::DepthMetaData dmd;
	g_DepthGenerator.GetMetaData(dmd);

	if (!g_bPause)
	{
		// Read next available data
		g_Context.WaitAndUpdateAll();
	}

	// Process the data
	g_DepthGenerator.GetMetaData(dmd);
	g_UserGenerator.GetUserPixels(0, smd);

	//static int texWidth, texHeight;
	//texWidth =  getClosestPowerOfTwo(dmd.XRes());
	//texHeight = getClosestPowerOfTwo(dmd.YRes());

	unsigned int nValue = 0;
	unsigned int nHistValue = 0;
	unsigned int nIndex = 0;
	unsigned int nX = 0;
	unsigned int nY = 0;
	unsigned int nNumberOfPoints = 0;
	XnUInt16 g_nXRes = dmd.XRes();
	XnUInt16 g_nYRes = dmd.YRes();

	unsigned char* pDestImage = pDepthBuf;

	const XnDepthPixel* pDepth = dmd.Data();
	const XnLabel* pLabels = smd.Data();

	// Calculate the accumulative histogram
	memset(g_pDepthHist, 0, MAX_DEPTH*sizeof(float));
	for (nY=0; nY<g_nYRes; nY++)
	{
		for (nX=0; nX<g_nXRes; nX++)
		{
			nValue = *pDepth;

			if (nValue != 0)
			{
				g_pDepthHist[nValue]++;
				nNumberOfPoints++;
			}

			pDepth++;
		}
	}

	for (nIndex=1; nIndex<MAX_DEPTH; nIndex++)
	{
		g_pDepthHist[nIndex] += g_pDepthHist[nIndex-1];
	}
	if (nNumberOfPoints)
	{
		for (nIndex=1; nIndex<MAX_DEPTH; nIndex++)
		{
			g_pDepthHist[nIndex] = (unsigned int)(256 * (1.0f - (g_pDepthHist[nIndex] / nNumberOfPoints)));
		}
	}

	pDepth = dmd.Data();
	if (g_bDrawPixels)
	{
		XnUInt32 nIndex = 0;
		// Prepare the texture map
		for (nY=0; nY<g_nYRes; nY++)
		{
			for (nX=0; nX < g_nXRes; nX++, nIndex++)
			{

				pDestImage[0] = 0;
				pDestImage[1] = 0;
				pDestImage[2] = 0;
				pDestImage[3] = 1;
				if (g_bDrawBackground || *pLabels != 0)
				{
					nValue = *pDepth;
					XnLabel label = *pLabels;
					XnUInt32 nColorID = label % nColors;
					if (label == 0)
					{
						nColorID = nColors;
					}

					if (nValue != 0)
					{
						nHistValue = g_pDepthHist[nValue];

						pDestImage[0] = nHistValue * Colors[nColorID][0]; 
						pDestImage[1] = nHistValue * Colors[nColorID][1];
						pDestImage[2] = nHistValue * Colors[nColorID][2];
						pDestImage[3] = 1;
					}
				}

				pDepth++;
				pLabels++;
				pDestImage+=4;
			}

			//pDestImage += (texWidth - g_nXRes) *4;
		}
	}
	else
	{
		xnOSMemSet(pDepthBuf, 0, 4*2*g_nXRes*g_nYRes);
	}

	XnUserID aUsers[15];
	XnUInt16 nUsers = 15;
	g_UserGenerator.GetUsers(aUsers, nUsers);
	XnBool bTracking = false;
	for (int i = 0; i < nUsers; ++i)
	{
		XnUserID player = aUsers[i];
		if (g_UserGenerator.GetSkeletonCap().IsTracking(player))
		{
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_TORSO, pJointPos[0]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_NECK, pJointPos[1]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_HEAD, pJointPos[2]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_SHOULDER, pJointPos[3]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_ELBOW, pJointPos[4]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_HAND, pJointPos[5]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_SHOULDER, pJointPos[6]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_ELBOW, pJointPos[7]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_HAND, pJointPos[8]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_HIP, pJointPos[9]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_KNEE, pJointPos[10]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_LEFT_FOOT, pJointPos[11]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_HIP, pJointPos[12]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_KNEE, pJointPos[13]);
			g_UserGenerator.GetSkeletonCap().GetSkeletonJointPosition(player, XN_SKEL_RIGHT_FOOT, pJointPos[14]);
			bTracking = true;
		}
	}
	g_bTracking = bTracking;
}
}
// これは、エクスポートされたクラスのコンストラクタです。
// クラス定義に関しては NiSimpleTracker.h を参照してください。
CNiSimpleTracker::CNiSimpleTracker()
{
	return;
}
