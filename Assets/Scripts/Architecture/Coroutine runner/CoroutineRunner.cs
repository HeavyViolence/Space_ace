using System;
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

            private static IEnumerator SubroutineRunner(Action routine, Func<bool> runCondition)
            {
                while (runCondition() == false) yield return null;

                routine();
            }

            public static Coroutine RunRoutine(IEnumerator routine)
            {
                if (routine is null) throw new EmptyRoutineRunAttemptException();

                return Instance.StartCoroutine(routine);
            }

            public static Coroutine RunRoutine(Action routine, Func<bool> runCondition)
            {
                if (routine is null || runCondition is null) throw new EmptyRoutineRunAttemptException();

                return Instance.StartCoroutine(SubroutineRunner(routine, runCondition));
            }

            public static void StopRoutine(Coroutine routine)
            {
                if (routine is null) throw new EmptyRoutineStopAttemptException();

                Instance.StopCoroutine(routine);
            }
        }
    }
}