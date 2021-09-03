Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.graph.models


	Public Interface GraphVectors(Of V, E)

		ReadOnly Property Graph As IGraph(Of V, E)

		Function numVertices() As Integer

		ReadOnly Property VectorSize As Integer

		Function getVertexVector(ByVal vertex As Vertex(Of V)) As INDArray

		Function getVertexVector(ByVal vertexIdx As Integer) As INDArray

		Function verticesNearest(ByVal vertexIdx As Integer, ByVal top As Integer) As Integer()

		Function similarity(ByVal vertex1 As Vertex(Of V), ByVal vertex2 As Vertex(Of V)) As Double

		Function similarity(ByVal vertexIdx1 As Integer, ByVal vertexIdx2 As Integer) As Double

	End Interface

End Namespace