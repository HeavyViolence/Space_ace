using System.Collections;
using UnityEngine;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class CoroutineRunner : MonoBehaviour
        {
            private static CoroutineRunner s_instance = null;

            private static CoroutineRunner Instance
            {
                get
                {
                    if (s_instance == null)
                    {
                        GameObject coroutineRunner = new("Coroutine runner");
                        s_instance = coroutineRunner.AddComponent<CoroutineRunner>();
                    }

                    return s_instance;
                }
            }

            public static Coroutine RunRoutine(IEnumerator routine)
            {
                if (routine is null)
                {
                    throw new EmptyRoutineRunAttemptException();
                }

                return Instance.StartCoroutine(routine);
            }

            public static void StopRoutine(Coroutine routine)
            {
                if (routine is null)
                {
                    throw new EmptyRoutineStopAttemptException();
                }

                Instance.StopCoroutine(routine);
            }
        }
    }
}