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

Namespace org.deeplearning4j.graph.models.embeddings

	Public Interface GraphVectorLookupTable

		''' <summary>
		'''The size of the vector representations
		''' </summary>
		Function vectorSize() As Integer

		''' <summary>
		''' Reset (randomize) the weights. </summary>
		Sub resetWeights()

		''' <summary>
		''' Conduct learning given a pair of vertices (in and out) </summary>
		Sub iterate(ByVal first As Integer, ByVal second As Integer)

		''' <summary>
		''' Get the vector for the vertex with index idx </summary>
		Function getVector(ByVal idx As Integer) As INDArray

		''' <summary>
		''' Set the learning rate </summary>
		WriteOnly Property LearningRate As Double

		''' <summary>
		''' Returns the number of vertices in the graph </summary>
		ReadOnly Property NumVertices As Integer

	End Interface

End Namespace