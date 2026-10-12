Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.models
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.graph.models.embeddings


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor public class GraphVectorsImpl<V, E> implements org.deeplearning4j.graph.models.GraphVectors<V, E>
	<Serializable>
	Public Class GraphVectorsImpl(Of V, E)
		Implements GraphVectors(Of V, E)

		Protected Friend graph As IGraph(Of V, E)
		Protected Friend lookupTable As GraphVectorLookupTable


		Public Overridable Function getGraph() As IGraph(Of V, E) Implements GraphVectors(Of V, E).getGraph
			Return graph
		End Function

		Public Overridable Function numVertices() As Integer Implements GraphVectors(Of V, E).numVertices
			Return lookupTable.NumVertices
		End Function

		Public Overridable ReadOnly Property VectorSize As Integer Implements GraphVectors(Of V, E).getVectorSize
			Get
				Return lookupTable.vectorSize()
			End Get
		End Property

		Public Overridable Function getVertexVector(ByVal vertex As Vertex(Of V)) As INDArray Implements GraphVectors(Of V, E).getVertexVector
			Return lookupTable.getVector(vertex.vertexID())
		End Function

		Public Overridable Function getVertexVector(ByVal vertexIdx As Integer) As INDArray Implements GraphVectors(Of V, E).getVertexVector
			Return lookupTable.getVector(vertexIdx)
		End Function

		Public Overridable Function verticesNearest(ByVal vertexIdx As Integer, ByVal top As Integer) As Integer() Implements GraphVectors(Of V, E).verticesNearest

			Dim vec As INDArray = lookupTable.getVector(vertexIdx).dup()
			Dim norm2 As Double = vec.norm2Number().doubleValue()


			Dim pq As New PriorityQueue(Of Pair(Of Double, Integer))(lookupTable.NumVertices, New PairComparator())

			Dim l1 As Level1 = Nd4j.BlasWrapper.level1()
			Dim i As Integer = 0
			Do While i < numVertices()
				If i = vertexIdx Then
					i += 1
					Continue Do
				End If

				Dim other As INDArray = lookupTable.getVector(i)
				Dim cosineSim As Double = l1.dot(vec.length(), 1.0, vec, other) / (norm2 * other.norm2Number().doubleValue())

				pq.add(New Pair(Of )(cosineSim, i))
				i += 1
			Loop

			Dim [out](top - 1) As Integer
			For i As Integer = 0 To top - 1
				[out](i) = pq.remove().getSecond()
			Next i

			Return [out]
		End Function

		Private Class PairComparator
			Implements IComparer(Of Pair(Of Double, Integer))

			Public Overridable Function Compare(ByVal o1 As Pair(Of Double, Integer), ByVal o2 As Pair(Of Double, Integer)) As Integer Implements IComparer(Of Pair(Of Double, Integer)).Compare
				Return -o1.First.CompareTo(o2.First)
			End Function
		End Class

		''' <summary>
		'''Returns the cosine similarity of the vector representations of two vertices in the graph </summary>
		''' <returns> Cosine similarity of two vertices </returns>
		Public Overridable Function similarity(ByVal vertex1 As Vertex(Of V), ByVal vertex2 As Vertex(Of V)) As Double Implements GraphVectors(Of V, E).similarity
			Return similarity(vertex1.vertexID(), vertex2.vertexID())
		End Function

		''' <summary>
		'''Returns the cosine similarity of the vector representations of two vertices in the graph,
		''' given the indices of these verticies </summary>
		''' <returns> Cosine similarity of two vertices </returns>
		Public Overridable Function similarity(ByVal vertexIdx1 As Integer, ByVal vertexIdx2 As Integer) As Double Implements GraphVectors(Of V, E).similarity
			If vertexIdx1 = vertexIdx2 Then
				Return 1.0
			End If

			Dim vector As INDArray = Transforms.unitVec(getVertexVector(vertexIdx1))
			Dim vector2 As INDArray = Transforms.unitVec(getVertexVector(vertexIdx2))
			Return Nd4j.BlasWrapper.dot(vector, vector2)
		End Function
	End Class

End Namespace