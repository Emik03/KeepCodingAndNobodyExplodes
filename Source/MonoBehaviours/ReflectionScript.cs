﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static KeepCoding.Logger;
using static System.Linq.Enumerable;
using static System.Reflection.BindingFlags;
using static System.String;
using static UnityEngine.Application;
using Object = UnityEngine.Object;

namespace KeepCoding
{
    /// <summary>
    /// Editor-only behaviour that gets values from <see cref="Component"/>s in real-time.
    /// </summary>
    public sealed class ReflectionScript : MonoBehaviour, ILog
    {
        private class NullableObject
        {
            internal NullableObject(object o) => value = o;

            internal readonly object value;
        }

        private enum Methods
        {
            Get,
            GetChild,
            GetParent,
            Find
        }

        private const BindingFlags Flags = Instance | Static | Public | NonPublic | FlattenHierarchy;

        private Component[] Components => _method switch
        {
            Methods.Get => GetComponents<Component>(),
            Methods.GetChild => GetComponentsInChildren<Component>(true),
            Methods.GetParent => GetComponentsInParent<Component>(true),
            Methods.Find => FindObjectsOfType<Component>(),
            _ => throw new NotImplementedException(),
        };

        private static IList<Tuple<Component, NullableObject>> Empty => Empty<Tuple<Component, NullableObject>>().ToList();

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier
        private bool _slowMode;
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier
        private int _limitResults;
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier
        private Methods _method;
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
#pragma warning disable IDE0044 // Add readonly modifier
        private string _variable;
#pragma warning restore IDE0044 // Add readonly modifier

        [SerializeField]
        private string[] _values;

        [SerializeField]
        private Object[] _components;

        private IEnumerable<object> _current = Empty<object>();

        private IList<Tuple<Component, NullableObject>> _members = Empty;

        private readonly Logger _logger = new Logger(nameof(ReflectionScript), true, false);

        private void OnValidate()
        {
            if (_variable is null)
                return;

            _members = Empty<Tuple<Component, NullableObject>>().ToList();

            foreach (var component in Components)
            {
                var tuple = component.ToTuple(GetDeepValue(component, _variable.Split('.')));

                if (tuple.Item2 is null)
                    continue;

                _members.Add(tuple);

                if (_limitResults.IsBetween(1, _members.Count))
                    break;
            }
        }

        private void Start()
        {
            if (isEditor)
            {
                StartCoroutine(Loop());
                return;
            }

            Self($"A {nameof(ReflectionScript)} showed up in-game! Automatically deleting component...");
            Destroy(this);
        }

        private static NullableObject GetDeepValue(in object instance, in string[] names)
        {
            var current = new NullableObject(instance);

            foreach (string name in names)
            {
                if (current is null)
                    return null;

                var type = current.value?.GetType();

                var vs = new[]
{
                    GetField(type, name, current.value),
                    GetProperty(type, name, current.value),
                };

                current = vs.All(o => o is null) ? null : new NullableObject(vs.First(o => o is { }).value);
            }

            return current;
        }

        /// <summary>
        /// Logs message, but formats it to be compliant with the Logfile Analyzer.
        /// </summary>
        /// <exception cref="UnrecognizedValueException"></exception>
        /// <param name="message">The message to log.</param>
        /// <param name="logType">The type of logging. Different logging types have different icons within the editor.</param>
        public void Log<T>(T message, LogType logType = LogType.Log) => _logger.Log(message, logType);

        /// <summary>
        /// Logs multiple entries, but formats it to be compliant with the Logfile Analyzer.
        /// </summary>
        /// <exception cref="UnrecognizedValueException"></exception>
        /// <param name="message">The message to log.</param>
        /// <param name="args">All of the arguments to embed into <paramref name="message"/>.</param>
        public void Log<T>(T message, params object[] args) => _logger.Log(message, args);

        /// <summary>
        /// Logs multiple entries to the console.
        /// </summary>
        /// <param name="logs">The array of logs to individual output into the console.</param>
        public void LogMultiple(params string[] logs) => _logger.LogMultiple(logs);

        private static NullableObject GetField(in Type type, in string name, in object instance)
        {
            var field = type?.GetField(name);

            return field is null ? null
                : new NullableObject(field.IsStatic ? field.GetValue(null)
                : field.GetValue(instance));
        }

        private static NullableObject GetProperty(in Type type, in string name, in object instance)
        {
            var property = type?.GetProperty(name);

            return property is null ? null
                : new NullableObject(property.GetAccessors(false).Any(x => x.IsStatic) ? property.GetValue(null, null)
                : property.GetValue(instance, null));
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitUntil(() => enabled);

                yield return new WaitForSecondsRealtime(_slowMode ? 1 : 0.1f);

                var objects = _members.Select(o => o.Item2.value);

                var split = objects.SplitBy(o => o is Object);

                _components = split.Item1
                    .ToArray()
                    .ConvertAll(o => (Object)o);

                _values = split.Item2
                    .ToArray()
                    .ConvertAll(o => o.UnwrapToString());

                if (_current.SequenceEqual(objects))
                    continue;

                _current = objects;

                if (!_current.IsNullOrEmpty())
                    _logger.Log($"{gameObject.name}, {_variable}\n{Join(", ", _values)}");
            }
        }
    }
}
