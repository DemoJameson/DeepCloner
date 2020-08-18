using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner.Helpers;

namespace Force.DeepCloner {
    public delegate bool? KnownTypesProcessor(Type type);
    public delegate object PostCloneProcessor(object sourceObj, object clonedObj, DeepCloneState deepCloneState);
    public delegate object PreCloneProcessor(object sourceObj, DeepCloneState deepCloneState);

    public static class DeepCloner {
        private static readonly List<KnownTypesProcessor> KnownTypesProcessors = new List<KnownTypesProcessor>();
        private static readonly List<PostCloneProcessor> PostCloneProcessors = new List<PostCloneProcessor>();
        private static readonly List<PreCloneProcessor> PreCloneProcessors = new List<PreCloneProcessor>();

        public static void AddPostCloneProcessor(PostCloneProcessor postCloneProcessor) {
            if (postCloneProcessor != null && !PostCloneProcessors.Contains(postCloneProcessor)) {
                PostCloneProcessors.Add(postCloneProcessor);
            }
        }

        public static bool RemovePostCloneProcessor(PostCloneProcessor postCloneProcessor) {
            if (postCloneProcessor != null && PostCloneProcessors.Contains(postCloneProcessor)) {
                return PostCloneProcessors.Remove(postCloneProcessor);
            }

            return false;
        }

        public static void ClearPostCloneProcessors() {
            PostCloneProcessors.Clear();
        }

        internal static object InvokePostCloneProcessor(object sourceObj, object clonedObj, DeepCloneState deepCloneState) {
            return PostCloneProcessors.Aggregate(clonedObj, (returnObj, processor) => processor(sourceObj, returnObj, deepCloneState));
        }

        public static void AddPreCloneProcessor(PreCloneProcessor preCloneProcessor) {
            if (preCloneProcessor != null && !PreCloneProcessors.Contains(preCloneProcessor)) {
                PreCloneProcessors.Add(preCloneProcessor);
            }
        }

        public static bool RemovePreCloneProcessor(PreCloneProcessor preCloneProcessor) {
            if (preCloneProcessor != null && PreCloneProcessors.Contains(preCloneProcessor)) {
                return PreCloneProcessors.Remove(preCloneProcessor);
            }

            return false;
        }

        public static void ClearPreCloneProcessors() {
            PreCloneProcessors.Clear();
        }

        internal static object InvokePreCloneProcessor(object sourceObj, DeepCloneState deepCloneState) {
            return PreCloneProcessors.Aggregate(sourceObj, (returnObj, processor) => processor(sourceObj, deepCloneState));
        }

        public static void AddKnownTypesProcessor(KnownTypesProcessor knownTypesProcessor) {
            if (knownTypesProcessor != null && !KnownTypesProcessors.Contains(knownTypesProcessor)) {
                KnownTypesProcessors.Add(knownTypesProcessor);
            }
        }

        public static bool RemoveKnownTypesProcessor(KnownTypesProcessor knownTypesProcessor) {
            if (knownTypesProcessor != null && KnownTypesProcessors.Contains(knownTypesProcessor)) {
                return KnownTypesProcessors.Remove(knownTypesProcessor);
            }

            return false;
        }

        public static void ClearKnownTypesProcessors() {
            KnownTypesProcessors.Clear();
        }

        // 不为 null 代表命中，中断执行
        internal static bool? InvokeKnownTypesProcessor(Type type) {
            foreach (KnownTypesProcessor processor in KnownTypesProcessors) {
                bool? result = processor(type);
                if (result != null) return result;
            }

            return null;
        }
    }
}