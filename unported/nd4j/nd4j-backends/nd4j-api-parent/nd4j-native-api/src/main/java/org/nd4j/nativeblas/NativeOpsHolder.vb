Imports System
Imports Getter = lombok.Getter
Imports Loader = org.bytedeco.javacpp.Loader
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports ReflectionUtils = org.nd4j.common.io.ReflectionUtils
Imports Nd4jContext = org.nd4j.context.Nd4jContext
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.nativeblas

	Public Class NativeOpsHolder
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(NativeOpsHolder))
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New NativeOpsHolder()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final NativeOps deviceNativeOps;
		Private ReadOnly deviceNativeOps As NativeOps

		Public Shared Function getCores(ByVal totals As Integer) As Integer
			' that's special case for Xeon Phi
			If totals >= 256 Then
				Return 64
			End If

			Dim ht_off As Integer = totals \ 2 ' we count off HyperThreading without any excuses
			If ht_off <= 4 Then
				Return 4 ' special case for Intel i5. and nobody likes i3 anyway
			End If

			If ht_off > 24 Then
				Dim rounds As Integer = 0
				Do While ht_off > 24 ' we loop until final value gets below 24 cores, since that's reasonable threshold as of 2016
					If ht_off > 24 Then
						ht_off \= 2 ' we dont' have any cpus that has higher number then 24 physical cores
						rounds += 1
					End If
				Loop
				' 20 threads is special case in this branch
				If ht_off = 20 AndAlso rounds < 2 Then
					ht_off \= 2
				End If
			Else ' low-core models are known, but there's a gap, between consumer cpus and xeons
				If ht_off <= 6 Then
					' that's more likely consumer-grade cpu, so leave this value alone
					Return ht_off
				Else
					If isOdd(ht_off) Then ' if that's odd number, it's final result
						Return ht_off
					End If

					' 20 threads & 16 threads are special case in this branch, where we go min value
					If ht_off = 20 OrElse ht_off = 16 Then
						ht_off \= 2
					End If
				End If
			End If
			Return ht_off
		End Function

		Private Shared Function isOdd(ByVal value As Integer) As Boolean
			Return (value Mod 2 <> 0)
		End Function

		Private Sub New()
			Try
				Dim props As Properties = Nd4jContext.Instance.Conf

				Dim name As String = System.getProperty(Nd4j.NATIVE_OPS, props.get(Nd4j.NATIVE_OPS).ToString())
				Dim nativeOpsClass As Type = ND4JClassLoading.loadClassByName(name).asSubclass(GetType(NativeOps))
				deviceNativeOps = ReflectionUtils.newInstance(nativeOpsClass)

				deviceNativeOps.initializeDevicesAndFunctions()
				Dim numThreads As Integer
				Dim numThreadsString As String = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.OMP_NUM_THREADS)
				If numThreadsString IsNot Nothing AndAlso numThreadsString.Length > 0 Then
					numThreads = Integer.Parse(numThreadsString)
					deviceNativeOps.OmpNumThreads = numThreads
				Else
					Dim cores As Integer = Loader.totalCores()
					Dim chips As Integer = Loader.totalChips()
					If chips > 0 AndAlso cores > 0 Then
						deviceNativeOps.OmpNumThreads = Math.Max(1, cores \ chips)
					Else
						deviceNativeOps.OmpNumThreads = getCores(Runtime.getRuntime().availableProcessors())
					End If
				End If
				'deviceNativeOps.setOmpNumThreads(4);

				Dim logInitProperty As String = System.getProperty(ND4JSystemProperties.LOG_INITIALIZATION, "true")
				Dim logInit As Boolean = Boolean.Parse(logInitProperty)

				If logInit Then
					log.info("Number of threads used for linear algebra: {}", deviceNativeOps.ompGetMaxThreads())
				End If
			Catch e As Exception When TypeOf e Is Exception OrElse TypeOf e Is Exception
				Throw New Exception("ND4J is probably missing dependencies. For more information, please refer to: https://deeplearning4j.konduit.ai/nd4j/backend", e)
			End Try
		End Sub

		Public Shared ReadOnly Property Instance As NativeOpsHolder
			Get
				Return INSTANCE_Conflict
			End Get
		End Property
	End Class

End Namespace