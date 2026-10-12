Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Environment = org.nd4j.linalg.factory.Environment
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resource = org.nd4j.common.io.Resource
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

Namespace org.nd4j.linalg.cpu.nativecpu

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuBackend extends org.nd4j.linalg.factory.Nd4jBackend
	Public Class CpuBackend
		Inherits Nd4jBackend


		Private Const LINALG_PROPS As String = "/nd4j-native.properties"

		Public Overrides ReadOnly Property Available As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function canRun() As Boolean
			'no reliable way (yet!) to determine if running
			Return True
		End Function

		Public Overrides Function allowsOrder() As Boolean
			Return False
		End Function

		Public Overrides ReadOnly Property Priority As Integer
			Get
				Return BACKEND_PRIORITY_CPU
			End Get
		End Property

		Public Overrides ReadOnly Property ConfigurationResource As Resource
			Get
				Return New ClassPathResource(LINALG_PROPS, GetType(CpuBackend).getClassLoader())
			End Get
		End Property

		Public Overrides ReadOnly Property NDArrayClass As Type
			Get
				Return GetType(NDArray)
			End Get
		End Property

		Public Overrides ReadOnly Property Environment As Environment
			Get
				Return CpuEnvironment.Instance
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
					log.info("Backend build information:" & vbLf & " {}", buildInfo())
				Catch t As Exception
					log.debug("Error logging CPU backend ", t)
				End Try
			End If
		End Sub

	End Class


End Namespace