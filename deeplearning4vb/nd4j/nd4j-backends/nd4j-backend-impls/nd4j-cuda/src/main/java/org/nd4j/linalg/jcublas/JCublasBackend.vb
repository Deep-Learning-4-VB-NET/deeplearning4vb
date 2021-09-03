Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Loader = org.bytedeco.javacpp.Loader
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports Environment = org.nd4j.linalg.factory.Environment
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resource = org.nd4j.common.io.Resource
Imports CudaEnvironment = org.nd4j.nativeblas.CudaEnvironment
Imports Nd4jCuda = org.nd4j.nativeblas.Nd4jCuda
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.linalg.jcublas


	''' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JCublasBackend extends org.nd4j.linalg.factory.Nd4jBackend
	Public Class JCublasBackend
		Inherits Nd4jBackend


		Private Const LINALG_PROPS As String = "/nd4j-jcublas.properties"


		Public Overrides ReadOnly Property Available As Boolean
			Get
				Try
					If Not canRun() Then
						Return False
					End If
				Catch e As Exception
					Do While e.getCause() IsNot Nothing
						e = e.getCause()
					Loop
					e.printStackTrace()
					Throw New Exception(e)
				End Try
				Return True
			End Get
		End Property

		Public Overrides Function canRun() As Boolean
			Dim count() As Integer = { 0 }
			org.bytedeco.cuda.global.cudart.cudaGetDeviceCount(count)
			If count(0) <= 0 Then
				Throw New Exception("No CUDA devices were found in system")
			End If
			Loader.load(GetType(org.bytedeco.cuda.global.cublas))

			Return True
		End Function

		Public Overrides Function allowsOrder() As Boolean
			Return False
		End Function

		Public Overrides ReadOnly Property Priority As Integer
			Get
				Return BACKEND_PRIORITY_GPU
			End Get
		End Property

		Public Overrides ReadOnly Property ConfigurationResource As Resource
			Get
				Return New ClassPathResource(LINALG_PROPS, GetType(JCublasBackend).getClassLoader())
			End Get
		End Property

		Public Overrides ReadOnly Property NDArrayClass As Type
			Get
				Return GetType(JCublasNDArray)
			End Get
		End Property

		Public Overrides ReadOnly Property Environment As Environment
			Get
				Return CudaEnvironment.Instance
			End Get
		End Property

		Public Overrides Function buildInfo() As String
			Return NativeOpsHolder.Instance.getDeviceNativeOps().buildInfo()
		End Function

		Public Overrides Sub logBackendInit()
			Dim logInitProperty As String = System.getProperty(ND4JSystemProperties.LOG_INITIALIZATION, "true")
			Dim logInit As Boolean = Boolean.Parse(logInitProperty)

			If logInit Then
				Try
					Dim e As Nd4jCuda.Environment = Nd4jCuda.Environment.getInstance()
					Dim blasMajor As Integer = e.blasMajorVersion()
					Dim blasMinor As Integer = e.blasMinorVersion()
					Dim blasPatch As Integer = e.blasPatchVersion()
					log.info("ND4J CUDA build version: {}.{}.{}", blasMajor, blasMinor, blasPatch)
					Dim nGPUs As Integer = Nd4jEnvironment.Environment.getNumGpus()

					Dim props As Properties = Nd4j.Executioner.EnvironmentInformation
					Dim devicesList As IList(Of IDictionary(Of String, Object)) = CType(props.get(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY), IList(Of IDictionary(Of String, Object)))

					For i As Integer = 0 To nGPUs - 1
						Dim dev As IDictionary(Of String, Object) = devicesList(i)
						Dim name As String = DirectCast(dev(Nd4jEnvironment.CUDA_DEVICE_NAME_KEY), String)
						Dim major As Integer = DirectCast(dev(Nd4jEnvironment.CUDA_DEVICE_MAJOR_VERSION_KEY), Number).intValue()
						Dim minor As Integer = DirectCast(dev(Nd4jEnvironment.CUDA_DEVICE_MINOR_VERSION_KEY), Number).intValue()
						Dim totalMem As Long = DirectCast(dev(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY), Number).longValue()
						log.info("CUDA device {}: [{}]; cc: [{}.{}]; Total memory: [{}]", i, name, major, minor, totalMem)
					Next i
					log.info("Backend build information:" & vbLf & " {}", buildInfo())
				Catch t As Exception
					log.debug("Error logging CUDA backend versions and devices", t)
				End Try
			End If
		End Sub
	End Class

End Namespace