Imports System.Collections.Generic
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
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


	Public Class Nd4jTestSuite
		'the system property for what backends should run
		Public Const BACKENDS_TO_LOAD As String = "backends"
		Private Shared BACKENDS As IList(Of Nd4jBackend) = New List(Of Nd4jBackend)()
		Shared Sub New()
			Dim loadedBackends As ServiceLoader(Of Nd4jBackend) = ND4JClassLoading.loadService(GetType(Nd4jBackend))
			For Each backend As Nd4jBackend In loadedBackends
				BACKENDS.Add(backend)
			Next backend
		End Sub



		''' <summary>
		''' Based on the jvm arguments, an empty list is returned
		''' if all backends should be run.
		''' If only certain backends should run, please
		''' pass a csv to the jvm as follows:
		''' -Dorg.nd4j.linalg.tests.backendstorun=your.class1,your.class2 </summary>
		''' <returns> the list of backends to run </returns>
		Public Shared Function backendsToRun() As IList(Of String)
			Dim ret As IList(Of String) = New List(Of String)()
			Dim val As String = System.getProperty(BACKENDS_TO_LOAD, "")
			If val.Length = 0 Then
				Return ret
			End If

			Dim clazzes() As String = val.Split(",", True)

			For Each s As String In clazzes
				ret.Add(s)
			Next s
			Return ret

		End Function



	End Class

End Namespace