Imports Environment = org.nd4j.linalg.factory.Environment

' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************
Namespace org.nd4j.nativeblas

	''' <summary>
	''' CUDA backend implementation of <seealso cref="Environment"/>
	''' 
	''' @author Alex Black
	''' </summary>
	Public Class CudaEnvironment
		Implements Environment


'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New CudaEnvironment(Nd4jCuda.Environment.getInstance())

		Private ReadOnly e As Nd4jCuda.Environment

		Public Shared ReadOnly Property Instance As CudaEnvironment
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Protected Friend Sub New(ByVal environment As Nd4jCuda.Environment)
			Me.e = environment
		End Sub

		Public Overridable Function blasMajorVersion() As Integer Implements Environment.blasMajorVersion
			Return e.blasMajorVersion()
		End Function

		Public Overridable Function blasMinorVersion() As Integer Implements Environment.blasMinorVersion
			Return e.blasMinorVersion()
		End Function

		Public Overridable Function blasPatchVersion() As Integer Implements Environment.blasPatchVersion
			Return e.blasMajorVersion()
		End Function

		Public Overridable Property Verbose As Boolean Implements Environment.isVerbose
			Get
				Return e.isVerbose()
			End Get
			Set(ByVal reallyVerbose As Boolean)
				e.setVerbose(reallyVerbose)
			End Set
		End Property


		Public Overridable Property Debug As Boolean Implements Environment.isDebug
			Get
				Return e.isDebug()
			End Get
			Set(ByVal reallyDebug As Boolean)
				e.setDebug(reallyDebug)
			End Set
		End Property

		Public Overridable Property Profiling As Boolean Implements Environment.isProfiling
			Get
				Return e.isProfiling()
			End Get
			Set(ByVal reallyProfile As Boolean)
				e.setProfiling(reallyProfile)
			End Set
		End Property

		Public Overridable ReadOnly Property DetectingLeaks As Boolean Implements Environment.isDetectingLeaks
			Get
				Return e.isDetectingLeaks()
			End Get
		End Property

		Public Overridable ReadOnly Property DebugAndVerbose As Boolean Implements Environment.isDebugAndVerbose
			Get
				Return e.isDebugAndVerbose()
			End Get
		End Property



		Public Overridable WriteOnly Property LeaksDetector Implements Environment.setLeaksDetector As Boolean
			Set(ByVal reallyDetect As Boolean)
				e.setLeaksDetector(reallyDetect)
			End Set
		End Property

		Public Overridable Function helpersAllowed() As Boolean Implements Environment.helpersAllowed
			Return e.helpersAllowed()
		End Function

		Public Overridable Sub allowHelpers(ByVal reallyAllow As Boolean) Implements Environment.allowHelpers
			e.allowHelpers(reallyAllow)
		End Sub

		Public Overridable Function tadThreshold() As Integer Implements Environment.tadThreshold
			Return e.tadThreshold()
		End Function

		Public Overridable WriteOnly Property TadThreshold Implements Environment.setTadThreshold As Integer
			Set(ByVal threshold As Integer)
				e.setTadThreshold(threshold)
			End Set
		End Property

		Public Overridable Function elementwiseThreshold() As Integer Implements Environment.elementwiseThreshold
			Return e.elementwiseThreshold()
		End Function

		Public Overridable WriteOnly Property ElementwiseThreshold Implements Environment.setElementwiseThreshold As Integer
			Set(ByVal threshold As Integer)
				e.setElementwiseThreshold(threshold)
			End Set
		End Property

		Public Overridable Function maxThreads() As Integer Implements Environment.maxThreads
			Return e.maxThreads()
		End Function

		Public Overridable WriteOnly Property MaxThreads Implements Environment.setMaxThreads As Integer
			Set(ByVal max As Integer)
				e.setMaxThreads(max)
			End Set
		End Property

		Public Overridable Function maxMasterThreads() As Integer Implements Environment.maxMasterThreads
			Return e.maxMasterThreads()
		End Function

		Public Overridable WriteOnly Property MaxMasterThreads Implements Environment.setMaxMasterThreads As Integer
			Set(ByVal max As Integer)
				e.setMaxMasterThreads(max)
			End Set
		End Property

		Public Overridable WriteOnly Property MaxPrimaryMemory Implements Environment.setMaxPrimaryMemory As Long
			Set(ByVal maxBytes As Long)
				e.setMaxPrimaryMemory(maxBytes)
			End Set
		End Property

		Public Overridable WriteOnly Property MaxSpecialMemory Implements Environment.setMaxSpecialMemory As Long
			Set(ByVal maxBytes As Long)
				e.setMaxSpecialyMemory(maxBytes)
			End Set
		End Property

		Public Overridable WriteOnly Property MaxDeviceMemory Implements Environment.setMaxDeviceMemory As Long
			Set(ByVal maxBytes As Long)
				e.setMaxDeviceMemory(maxBytes)
			End Set
		End Property

		Public Overridable ReadOnly Property CPU As Boolean Implements Environment.isCPU
			Get
				Return e.isCPU()
			End Get
		End Property

		Public Overridable Sub setGroupLimit(ByVal group As Integer, ByVal numBytes As Long) Implements Environment.setGroupLimit
			e.setGroupLimit(group, numBytes)
		End Sub

		Public Overridable Sub setDeviceLimit(ByVal deviceId As Integer, ByVal numBytes As Long) Implements Environment.setDeviceLimit
			e.setDeviceLimit(deviceId, numBytes)
		End Sub

		Public Overridable Function getGroupLimit(ByVal group As Integer) As Long Implements Environment.getGroupLimit
			Return e.getGroupLimit(group)
		End Function

		Public Overridable Function getDeviceLimit(ByVal deviceId As Integer) As Long Implements Environment.getDeviceLimit
			Return e.getDeviceLimit(deviceId)
		End Function

		Public Overridable Function getDeviceCouner(ByVal deviceId As Integer) As Long Implements Environment.getDeviceCouner
			Return e.getDeviceCounter(deviceId)
		End Function
	End Class

End Namespace