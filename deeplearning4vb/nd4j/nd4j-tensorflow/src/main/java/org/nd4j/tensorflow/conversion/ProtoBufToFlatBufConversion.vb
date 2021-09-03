Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports org.nd4j.autodiff.samediff.transform
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports TFImportOverride = org.nd4j.imports.tensorflow.TFImportOverride
Imports TFOpImportFilter = org.nd4j.imports.tensorflow.TFOpImportFilter
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.tensorflow.conversion


	Public Class ProtoBufToFlatBufConversion

		''' <summary>
		''' Converts a file containing a model from the Protocol Buffer format to the Flat
		''' Buffer format. </summary>
		''' <param name="inFile"> input file (.pb format) </param>
		''' <param name="outFile"> output file (.fb format) </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="org.nd4j.linalg.exception.ND4JIllegalStateException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convert(String inFile, String outFile) throws IOException, org.nd4j.linalg.exception.ND4JIllegalStateException
		Public Shared Sub convert(ByVal inFile As String, ByVal outFile As String)
			Dim tg As SameDiff = TFGraphMapper.importGraph(New File(inFile))
			tg.asFlatFile(New File(outFile))
		End Sub

		''' <summary>
		''' Converts a BERT model from the Protocol Buffer format to the Flat Buffer format. </summary>
		''' <param name="inFile"> input file (.pb format) </param>
		''' <param name="outFile"> output file (.fb format) </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="org.nd4j.linalg.exception.ND4JIllegalStateException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convertBERT(String inFile, String outFile) throws IOException, org.nd4j.linalg.exception.ND4JIllegalStateException
		Public Shared Sub convertBERT(ByVal inFile As String, ByVal outFile As String)
			'
			' Working around some issues in the BERT model's execution. See file:
			' nd4j/nd4j-backends/nd4j-tests/src/test/java/org/nd4j/imports/TFGraphs/BERTGraphTest.java
			' for details.

			Dim minibatchSize As Integer = 4
			Dim m As IDictionary(Of String, TFImportOverride) = New Dictionary(Of String, TFImportOverride)()
			m("IteratorGetNext") = Function(inputs, controlDepInputs, nodeDef, initWith, attributesForNode, graph)
			Return java.util.Arrays.asList(initWith.placeHolder("IteratorGetNext", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:1", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:4", DataType.INT, minibatchSize, 128))
			End Function

			' Skip the "IteratorV2" op - we don't want or need this
			Dim filter As TFOpImportFilter = Function(nodeDef, initWith, attributesForNode, graph)
			Return "IteratorV2".Equals(nodeDef.getName())
			End Function


			Dim sd As SameDiff = TFGraphMapper.importGraph(New File(inFile), m, filter)


			Dim p As SubGraphPredicate = SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/mul")).withInputCount(2).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/div"))).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/Floor")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/add")).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/mul")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/RandomUniform"))).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/sub")))))))

			Dim subGraphs As IList(Of SubGraph) = GraphTransformUtil.getSubgraphsMatching(sd, p)
			Dim subGraphCount As Integer = subGraphs.Count
			sd = GraphTransformUtil.replaceSubgraphsMatching(sd, p, New SubGraphProcessorAnonymousInnerClass())


			Console.WriteLine("Exporting file " & outFile)
			sd.asFlatFile(New File(outFile))
		End Sub

		Private Class SubGraphProcessorAnonymousInnerClass
			Implements SubGraphProcessor

			Public Function processSubgraph(ByVal sd As SameDiff, ByVal subGraph As SubGraph) As IList(Of SDVariable)
				Dim inputs As IList(Of SDVariable) = subGraph.inputs() ' Get inputs to the subgraph
				' Find pre-dropout input variable:
				Dim newOut As SDVariable = Nothing
				For Each v As SDVariable In inputs
					If v.VarName.EndsWith("/BiasAdd", StringComparison.Ordinal) OrElse v.VarName.EndsWith("/Softmax", StringComparison.Ordinal) OrElse v.VarName.EndsWith("/add_1", StringComparison.Ordinal) OrElse v.VarName.EndsWith("/Tanh", StringComparison.Ordinal) Then
						newOut = v
						Exit For
					End If
				Next v

				If newOut IsNot Nothing Then
					' Pass this input variable as the new output
					Return Collections.singletonList(newOut)
				End If

				Throw New Exception("No pre-dropout input variable found")
			End Function
		End Class


		''' <summary>
		''' Main function.
		''' The conversion tool can be called from the command line with the floowing syntax:
		''' mvn exec:java -Dexec.mainClass="org.nd4j.tensorflow.conversion.ProtoBufToFlatBufConversion" -Dexec.args="<input_file.pb> <output_file.fb>"
		''' </summary>
		''' <param name="args"> the first argument is the input filename (protocol buffer format),
		'''             the second one is the output filename (flat buffer format) </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String[] args) throws java.io.IOException
		Public Shared Sub Main(ByVal args() As String)
			If args.Length < 2 Then
				Console.Error.WriteLine("Usage:" & vbLf & "mvn exec:java -Dexec.mainClass=""org.nd4j.tensorflow.conversion.ProtoBufToFlatBufConversion"" -Dexec.args=""<input_file.pb> <output_file.fb>""" & vbLf)
			Else
				convert(args(0), args(1))
			End If
		End Sub

	End Class

End Namespace