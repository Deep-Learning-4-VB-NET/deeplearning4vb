Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.imports.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled public class ImportModelDebugger
	Public Class ImportModelDebugger
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void doTest()
		Public Overridable Sub doTest()
			main(New String(){})
		End Sub

		Public Shared Sub Main(ByVal args() As String)

			Dim modelFile As New File("C:\Temp\TF_Graphs\cifar10_gan_85\tf_model.pb")
			Dim rootDir As New File("C:\Temp\TF_Graphs\cifar10_gan_85")

			Dim sd As SameDiff = TFGraphMapper.importGraph(modelFile)

			Dim l As ImportDebugListener = ImportDebugListener.builder(rootDir).checkShapesOnly(True).floatingPointEps(1e-5).onFailure(ImportDebugListener.OnFailure.EXCEPTION).logPass(True).build()

			sd.setListeners(l)

			Dim ph As IDictionary(Of String, INDArray) = loadPlaceholders(rootDir)

			Dim outputs As IList(Of String) = sd.outputs()

			sd.output(ph, outputs)
		End Sub


		Public Shared Function loadPlaceholders(ByVal rootDir As File) As IDictionary(Of String, INDArray)
			Dim dir As New File(rootDir, "__placeholders")
			If Not dir.exists() Then
				Throw New System.InvalidOperationException("Cannot find placeholders: directory does not exist: " & dir.getAbsolutePath())
			End If

			Dim ret As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim iter As IEnumerator(Of File) = FileUtils.iterateFiles(dir, Nothing, True)
			Do While iter.MoveNext()
				Dim f As File = iter.Current
				If Not f.isFile() Then
					Continue Do
				End If
				Dim s As String = dir.toURI().relativize(f.toURI()).getPath()
				Dim idx As Integer = s.LastIndexOf("__", StringComparison.Ordinal)
				Dim name As String = s.Substring(0, idx)
				Dim arr As INDArray = Nd4j.createFromNpyFile(f)
				ret(name) = arr
			Loop

			Return ret
		End Function
	End Class

End Namespace