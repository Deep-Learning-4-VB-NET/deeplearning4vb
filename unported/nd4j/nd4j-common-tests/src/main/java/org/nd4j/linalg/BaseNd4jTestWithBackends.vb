Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ReflectionUtils = org.nd4j.common.io.ReflectionUtils
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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

Namespace org.nd4j.linalg


	''' <summary>
	''' Base Nd4j test
	''' @author Adam Gibson
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseNd4jTestWithBackends extends org.nd4j.common.tests.BaseND4JTest
	Public MustInherit Class BaseNd4jTestWithBackends
		Inherits BaseND4JTest

		Public Shared BACKENDS As IList(Of Nd4jBackend) = New List(Of Nd4jBackend)()
		Shared Sub New()
			Dim backendsToRun As IList(Of String) = Nd4jTestSuite.backendsToRun()

			Dim loadedBackends As ServiceLoader(Of Nd4jBackend) = ND4JClassLoading.loadService(GetType(Nd4jBackend))
			For Each backend As Nd4jBackend In loadedBackends
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				If backend.canRun() AndAlso backendsToRun.Contains(backend.GetType().FullName) OrElse backendsToRun.Count = 0 Then
					BACKENDS.Add(backend)
				End If
			Next backend
		End Sub

		Public Const DEFAULT_BACKEND As String = "org.nd4j.linalg.defaultbackend"



		Public Shared Function configs() As Stream(Of Arguments)
			Dim ret As Stream(Of Arguments) = BACKENDS.Select(Function(input) Arguments.of(input))
			Return ret
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void beforeTest2()
		Public Overridable Sub beforeTest2()
			Nd4j.factory().Order = ordering()
		End Sub

		''' <summary>
		''' Get the default backend (nd4j)
		''' The default backend can be overridden by also passing:
		''' -Dorg.nd4j.linalg.defaultbackend=your.backend.classname </summary>
		''' <returns> the default backend based on the
		''' given command line arguments </returns>
		Public Shared ReadOnly Property DefaultBackend As Nd4jBackend
			Get
				Dim cpuBackend As String = "org.nd4j.linalg.cpu.nativecpu.CpuBackend"
				Dim defaultBackendClass As String = System.getProperty(DEFAULT_BACKEND, cpuBackend)
    
				Dim backendClass As Type(Of Nd4jBackend) = ND4JClassLoading.loadClassByName(defaultBackendClass)
				Return ReflectionUtils.newInstance(backendClass)
			End Get
		End Property

		''' <summary>
		''' The ordering for this test
		''' This test will only be invoked for
		''' the given test  and ignored for others
		''' </summary>
		''' <returns> the ordering for this test </returns>
		Public Overridable Function ordering() As Char
			Return "c"c
		End Function

		Public Overridable Function getFailureMessage(ByVal backend As Nd4jBackend) As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return "Failed with backend " & backend.GetType().FullName & " and ordering " & ordering()
		End Function
	End Class

End Namespace