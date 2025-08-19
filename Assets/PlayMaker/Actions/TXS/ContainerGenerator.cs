using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TXS")]
	public class ContainerGenerator : FsmStateAction
	{
        [RequiredField]
        [Tooltip("Root Cube to modify")]
        public FsmGameObject TargetCubeFsm;

        [RequiredField]
        [Tooltip("Numerator value")]
        public FsmInt NumeratorFsm;

        [RequiredField]
        [Tooltip("Denominator value")]
        public FsmInt DenominatorFsm;

        [Tooltip("Denominator value")]
        public bool IsPositive = true;

        [RequiredField]
        [Tooltip("Original Target Cube to save")]
        public FsmGameObject TargeCubeOriginal;

        //public Vector3 SectionCount;
        public Material DenominatorMaterial;
        public Material NumeratorMaterial;

        private Vector3 SizeOfOriginalCube;
        private Vector3 SectionSize;
        private Vector3 FillStartPosition;
        private Transform ParentTransform;
        private GameObject SubCube;

        private GameObject TargetCube;
        private int Numerator;
        private int Denominator;
        private GameObject OldMinicube;
        

        void Start()
        {
            
        }
        
        // Code that runs on entering the state.
        public override void OnEnter()
		{
            TargeCubeOriginal = TargetCubeFsm;
            InstantCubifier(NumeratorFsm,DenominatorFsm);
			Finish();
		}

        public void InstantCubifier(FsmInt NumeratorFsm, FsmInt DenominatorFsm)
        {
            

            TargetCube = TargeCubeOriginal.Value;
            Numerator = NumeratorFsm.Value;
            Denominator = DenominatorFsm.Value;

            if (!IsPositive) Denominator = Denominator - 1;
            else Denominator = Denominator + 1;
            

            if (Denominator >= 1)
            {
                DenominatorFsm.Value = Denominator;

                OldMinicube = GameObject.Find(TargetCube.name + "CubeParent");
                if (OldMinicube != null)
                {
                    GameObject.Destroy(OldMinicube);
                }


                SizeOfOriginalCube = TargetCube.transform.lossyScale;
                SectionSize = new Vector3(
                    SizeOfOriginalCube.x / 1,
                    SizeOfOriginalCube.y / Denominator,
                    SizeOfOriginalCube.z / 1
                    );

                //FillStartPosition = TargetCube.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f))
                //                    + TargetCube.transform.TransformDirection(new Vector3(SectionSize.x, -SectionSize.y, SectionSize.z) / 2.0f);

                FillStartPosition = TargetCube.transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.5f))
                                    + TargetCube.transform.TransformDirection(new Vector3(-SectionSize.x, SectionSize.y, -SectionSize.z) / 2.0f);

                ParentTransform = new GameObject(TargetCube.name + "CubeParent").transform;

                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < Denominator; j++)
                    {
                        for (int k = 0; k < 1; k++)
                        {

                            SubCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            SubCube.transform.localScale = SectionSize;
                            SubCube.transform.position = FillStartPosition +
                                                            TargetCube.transform.TransformDirection(new Vector3((SectionSize.x) * i, (SectionSize.y) * j, (SectionSize.z) * k));
                            SubCube.transform.rotation = TargetCube.transform.rotation;

                            SubCube.transform.SetParent(ParentTransform);
                            //SubCube.GetComponent<MeshRenderer>().material = SubCubeMaterial;

                            if (j < Numerator)
                            {
                                SubCube.GetComponent<MeshRenderer>().material = NumeratorMaterial;
                            }
                            else
                            {
                                SubCube.GetComponent<MeshRenderer>().material = DenominatorMaterial;
                            }

                        }
                    }
                }
                //GameObject.Destroy(TargetCube);
                TargetCube.SetActive(false);

            }
        }

    }

}
