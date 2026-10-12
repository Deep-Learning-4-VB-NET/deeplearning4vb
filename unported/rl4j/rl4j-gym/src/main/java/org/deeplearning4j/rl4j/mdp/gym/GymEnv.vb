Imports System
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports SizeTPointer = org.bytedeco.javacpp.SizeTPointer
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.space
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports HighLowDiscrete = org.deeplearning4j.rl4j.space.HighLowDiscrete
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.space
Imports org.bytedeco.cpython
Imports org.bytedeco.numpy
Imports org.bytedeco.cpython.global.python
Imports org.bytedeco.numpy.global.numpy

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.rl4j.mdp.gym

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class GymEnv<OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable, A, @AS extends org.deeplearning4j.rl4j.space.ActionSpace<A>> implements org.deeplearning4j.rl4j.mdp.MDP<OBSERVATION, A, @AS>
	Public Class GymEnv(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, A, [AS] As org.deeplearning4j.rl4j.space.ActionSpace(Of A))
		Implements MDP(Of OBSERVATION, A, [AS])

		Public Const GYM_MONITOR_DIR As String = "/tmp/gym-dqn"

		Private Shared Sub checkPythonError()
			If PyErr_Occurred() IsNot Nothing Then
				PyErr_Print()
				Throw New Exception("Python error occurred")
			End If
		End Sub

		Private Shared program As Pointer
		Private Shared globals As PyObject
		Shared Sub New()
			Try
				Py_AddPath(org.bytedeco.gym.presets.gym.cachePackages())
				program = Py_DecodeLocale(GetType(GymEnv).Name, Nothing)
				Py_SetProgramName(program)
				Py_Initialize()
				PyEval_InitThreads()
				PySys_SetArgvEx(1, program, 0)
				If _import_array() < 0 Then
					PyErr_Print()
					Throw New Exception("numpy.core.multiarray failed to import")
				End If
				globals = PyModule_GetDict(PyImport_AddModule("__main__"))
				PyEval_SaveThread() ' just to release the GIL
			Catch e As IOException
				PyMem_RawFree(program)
				Throw New Exception(e)
			End Try
		End Sub
		Private locals As PyObject

'JAVA TO VB CONVERTER NOTE: The field actionSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly actionSpace_Conflict As DiscreteSpace
		Protected Friend ReadOnly observationSpace As ObservationSpace(Of OBSERVATION)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final String envId;
		Private ReadOnly envId As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final boolean render;
		Private ReadOnly render As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final boolean monitor;
		Private ReadOnly monitor As Boolean
		Private actionTransformer As ActionTransformer = Nothing
'JAVA TO VB CONVERTER NOTE: The field done was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private done_Conflict As Boolean = False

		Public Sub New(ByVal envId As String, ByVal render As Boolean, ByVal monitor As Boolean)
			Me.New(envId, render, monitor, DirectCast(Nothing, Integer?))
		End Sub
		Public Sub New(ByVal envId As String, ByVal render As Boolean, ByVal monitor As Boolean, ByVal seed As Integer?)
			Me.envId = envId
			Me.render = render
			Me.monitor = monitor

			Dim gstate As Integer = PyGILState_Ensure()
			Try
				locals = PyDict_New()

				Py_DecRef(PyRun_StringFlags("import gym; env = gym.make('" & envId & "')", Py_single_input, globals, locals, Nothing))
				checkPythonError()
				If monitor Then
					Py_DecRef(PyRun_StringFlags("env = gym.wrappers.Monitor(env, '" & GYM_MONITOR_DIR & "')", Py_single_input, globals, locals, Nothing))
					checkPythonError()
				End If
				If seed IsNot Nothing Then
					Py_DecRef(PyRun_StringFlags("env.seed(" & seed & ")", Py_single_input, globals, locals, Nothing))
					checkPythonError()
				End If
				Dim shapeTuple As PyObject = PyRun_StringFlags("env.observation_space.shape", Py_eval_input, globals, locals, Nothing)
				Dim shape(CInt(Math.Truncate(PyTuple_Size(shapeTuple))) - 1) As Integer
				For i As Integer = 0 To shape.Length - 1
					shape(i) = CInt(Math.Truncate(PyLong_AsLong(PyTuple_GetItem(shapeTuple, i))))
				Next i
				observationSpace = CType(New ArrayObservationSpace(Of Box)(shape), ObservationSpace(Of OBSERVATION))
				Py_DecRef(shapeTuple)

				Dim n As PyObject = PyRun_StringFlags("env.action_space.n", Py_eval_input, globals, locals, Nothing)
				actionSpace_Conflict = New DiscreteSpace(CInt(Math.Truncate(PyLong_AsLong(n))))
				Py_DecRef(n)
				checkPythonError()
			Finally
				PyGILState_Release(gstate)
			End Try
		End Sub

		Public Sub New(ByVal envId As String, ByVal render As Boolean, ByVal monitor As Boolean, ByVal actions() As Integer)
			Me.New(envId, render, monitor, Nothing, actions)
		End Sub
		Public Sub New(ByVal envId As String, ByVal render As Boolean, ByVal monitor As Boolean, ByVal seed As Integer?, ByVal actions() As Integer)
			Me.New(envId, render, monitor, seed)
			actionTransformer = New ActionTransformer(CType(ActionSpace, HighLowDiscrete), actions)
		End Sub

		Public Overridable Function getObservationSpace() As ObservationSpace(Of OBSERVATION) Implements MDP(Of OBSERVATION, A, [AS]).getObservationSpace
			Return observationSpace
		End Function

		Public Overridable ReadOnly Property ActionSpace As [AS]
			Get
				If actionTransformer Is Nothing Then
					Return CType(actionSpace_Conflict, [AS])
				Else
					Return CType(actionTransformer, [AS])
				End If
			End Get
		End Property

		Public Overridable Function [step](ByVal action As A) As StepReply(Of OBSERVATION)
			Dim gstate As Integer = PyGILState_Ensure()
			Try
				If render Then
					Py_DecRef(PyRun_StringFlags("env.render()", Py_single_input, globals, locals, Nothing))
					checkPythonError()
				End If
				Py_DecRef(PyRun_StringFlags("state, reward, done, info = env.step(" & CType(action, Integer?) & ")", Py_single_input, globals, locals, Nothing))
				checkPythonError()

				Dim state As New PyArrayObject(PyDict_GetItemString(locals, "state"))
				Dim stateData As DoublePointer = (New DoublePointer(PyArray_BYTES(state))).capacity(PyArray_Size(state))
				Dim stateDims As SizeTPointer = PyArray_DIMS(state).capacity(PyArray_NDIM(state))

				Dim reward As Double = PyFloat_AsDouble(PyDict_GetItemString(locals, "reward"))
				done_Conflict = PyLong_AsLong(PyDict_GetItemString(locals, "done")) <> 0
				checkPythonError()

				Dim data(CInt(stateData.capacity()) - 1) As Double
				stateData.get(data)

				Return New StepReply(New Box(data), reward, done_Conflict, Nothing)
			Finally
				PyGILState_Release(gstate)
			End Try
		End Function

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of OBSERVATION, A, [AS]).isDone
			Get
				Return done_Conflict
			End Get
		End Property

		Public Overridable Function reset() As OBSERVATION Implements MDP(Of OBSERVATION, A, [AS]).reset
			Dim gstate As Integer = PyGILState_Ensure()
			Try
				Py_DecRef(PyRun_StringFlags("state = env.reset()", Py_single_input, globals, locals, Nothing))
				checkPythonError()

				Dim state As New PyArrayObject(PyDict_GetItemString(locals, "state"))
				Dim stateData As DoublePointer = (New DoublePointer(PyArray_BYTES(state))).capacity(PyArray_Size(state))
				Dim stateDims As SizeTPointer = PyArray_DIMS(state).capacity(PyArray_NDIM(state))
				checkPythonError()

				done_Conflict = False

				Dim data(CInt(stateData.capacity()) - 1) As Double
				stateData.get(data)
				Return CType(New Box(data), OBSERVATION)
			Finally
				PyGILState_Release(gstate)
			End Try
		End Function

		Public Overridable Sub close() Implements MDP(Of OBSERVATION, A, [AS]).close
			Dim gstate As Integer = PyGILState_Ensure()
			Try
				Py_DecRef(PyRun_StringFlags("env.close()", Py_single_input, globals, locals, Nothing))
				checkPythonError()
				Py_DecRef(locals)
			Finally
				PyGILState_Release(gstate)
			End Try
		End Sub

		Public Overridable Function newInstance() As GymEnv(Of OBSERVATION, A, [AS])
			Return New GymEnv(Of OBSERVATION, A, [AS])(envId, render, monitor)
		End Function
	End Class

End Namespace