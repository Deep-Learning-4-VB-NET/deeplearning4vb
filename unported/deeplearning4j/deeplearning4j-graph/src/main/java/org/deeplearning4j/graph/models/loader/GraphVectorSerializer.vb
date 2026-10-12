Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports org.deeplearning4j.graph.models
Imports org.deeplearning4j.graph.models.deepwalk
Imports org.deeplearning4j.graph.models.embeddings
Imports InMemoryGraphLookupTable = org.deeplearning4j.graph.models.embeddings.InMemoryGraphLookupTable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.graph.models.loader


	Public Class GraphVectorSerializer
		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(GraphVectorSerializer))
		Private Shared ReadOnly DELIM As String = vbTab

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeGraphVectors(org.deeplearning4j.graph.models.deepwalk.DeepWalk deepWalk, String path) throws IOException
		Public Shared Sub writeGraphVectors(ByVal deepWalk As DeepWalk, ByVal path As String)

			Dim nVertices As Integer = deepWalk.numVertices()
			Dim vectorSize As Integer = deepWalk.getVectorSize()

			Using write As New StreamWriter(New StreamWriter(New File(path), False))
				For i As Integer = 0 To nVertices - 1
					Dim sb As New StringBuilder()
					sb.Append(i)
					Dim vec As INDArray = deepWalk.getVertexVector(i)
					For j As Integer = 0 To vectorSize - 1
						Dim d As Double = vec.getDouble(j)
						sb.Append(DELIM).Append(d)
					Next j
					sb.Append(vbLf)
					write.Write(sb.ToString())
				Next i
			End Using

			log.info("Wrote {} vectors of length {} to: {}", nVertices, vectorSize, path)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.graph.models.GraphVectors loadTxtVectors(File file) throws IOException
		Public Shared Function loadTxtVectors(ByVal file As File) As GraphVectors

			Dim vectorList As IList(Of Double()) = New List(Of Double())()

			Using reader As New StreamReader(file)
				Dim iter As LineIterator = IOUtils.lineIterator(reader)

				Do While iter.hasNext()
					Dim line As String = iter.next()
					Dim split() As String = line.Split(DELIM, True)
					Dim vec(split.Length - 2) As Double
					For i As Integer = 1 To split.Length - 1
						vec(i - 1) = Double.Parse(split(i))
					Next i
					vectorList.Add(vec)
				Loop
			End Using

			Dim vecSize As Integer = vectorList(0).Length
			Dim nVertices As Integer = vectorList.Count

			Dim vectors As INDArray = Nd4j.create(nVertices, vecSize)
			For i As Integer = 0 To vectorList.Count - 1
				Dim vec() As Double = vectorList(i)
				For j As Integer = 0 To vec.Length - 1
					vectors.put(i, j, vec(j))
				Next j
			Next i

			Dim table As New InMemoryGraphLookupTable(nVertices, vecSize, Nothing, 0.01)
			table.VertexVectors = vectors

			Return New GraphVectorsImpl(Of )(Nothing, table)
		End Function

	End Class

End Namespace