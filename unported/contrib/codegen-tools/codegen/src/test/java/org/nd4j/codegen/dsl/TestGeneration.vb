Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NamespaceOps = org.nd4j.codegen.api.NamespaceOps
Imports Nd4jNamespaceGenerator = org.nd4j.codegen.impl.java.Nd4jNamespaceGenerator
Imports RNNKt = org.nd4j.codegen.ops.RNNKt

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen.dsl


	Friend Class TestGeneration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") @TempDir public java.io.File testDir;
		Public testDir As File

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test()
			Dim f As File = testDir

	'        List<NamespaceOps> list = Arrays.asList(BitwiseKt.Bitwise(), RandomKt.Random());
			Dim list As IList(Of NamespaceOps) = New List(Of NamespaceOps) From {RNNKt.SDRNN()}

			For Each ops As NamespaceOps In list
				Nd4jNamespaceGenerator.generate(ops, Nothing, f, ops.getName() & ".java", "org.nd4j.linalg.factory", StringUtils.EMPTY)
			Next ops

			Dim files() As File = f.listFiles()
			Dim iter As IEnumerator(Of File) = FileUtils.iterateFiles(f, Nothing, True)
			If files IsNot Nothing Then
				Do While iter.MoveNext()
					Dim file As File = iter.Current
					If file.isDirectory() Then
						Continue Do
					End If
					Console.WriteLine(FileUtils.readFileToString(file, StandardCharsets.UTF_8))
					Console.WriteLine(vbLf & vbLf & "================" & vbLf & vbLf)
				Loop
			End If
		End Sub

	End Class

End Namespace