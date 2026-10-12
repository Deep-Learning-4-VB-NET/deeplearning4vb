Imports System
Imports System.Collections.Generic
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class DeconvTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DeconvTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void compareKeras(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub compareKeras(ByVal backend As Nd4jBackend)
			Dim newFolder As File = testDir.toFile()
			Call (New ClassPathResource("keras/deconv/")).copyDirectory(newFolder)

			Dim files() As File = newFolder.listFiles()

			Dim tests As ISet(Of String) = New HashSet(Of String)()
			For Each file As File In files
				Dim n As String = file.getName()
				If Not n.StartsWith("mb", StringComparison.Ordinal) Then
					Continue For
				End If

				Dim idx As Integer = n.LastIndexOf("_"c)
				Dim name As String = n.Substring(0, idx)
				tests.Add(name)
			Next file

			Dim l As IList(Of String) = New List(Of String)(tests)
			l.Sort()
			assertFalse(l.Count = 0)

			For Each s As String In l
				Dim s2 As String = s.replaceAll("[a-zA-Z]", "")
				Dim nums() As String = s2.Split("_", True)
				Dim mb As Integer = Integer.Parse(nums(0))
				Dim k As Integer = Integer.Parse(nums(1))
				Dim size As Integer = Integer.Parse(nums(2))
				Dim stride As Integer = Integer.Parse(nums(3))
				Dim same As Boolean = s.Contains("same")
				Dim d As Integer = Integer.Parse(nums(5))
				Dim nchw As Boolean = s.Contains("nchw")

				Dim w As INDArray = Nd4j.readNpy(New File(newFolder, s & "_W.npy"))
				Dim b As INDArray = Nd4j.readNpy(New File(newFolder, s & "_b.npy"))
				Dim [in] As INDArray = Nd4j.readNpy(New File(newFolder, s & "_in.npy")).castTo(DataType.FLOAT)
				Dim expOut As INDArray = Nd4j.readNpy(New File(newFolder, s & "_out.npy"))

				Dim op As CustomOp = DynamicCustomOp.builder("deconv2d").addInputs([in], w, b).addIntegerArguments(k, k, stride, stride, 0, 0, d, d,If(same, 1, 0),If(nchw, 0, 1)).callInplace(False).build()
				Dim [out] As INDArray = Nd4j.create(op.calculateOutputShape()(0))
				[out].assign(Double.NaN)
				op.addOutputArgument([out])
				Nd4j.exec(op)

				Dim eq As Boolean = expOut.equalsWithEps([out], 1e-4)
				assertTrue(eq)
			Next s
		End Sub
	End Class

End Namespace