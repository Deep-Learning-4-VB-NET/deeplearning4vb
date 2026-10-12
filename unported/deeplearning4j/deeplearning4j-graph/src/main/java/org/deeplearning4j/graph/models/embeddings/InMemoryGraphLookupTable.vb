Imports FastMath = org.apache.commons.math3.util.FastMath
Imports BinaryTree = org.deeplearning4j.graph.models.BinaryTree
Imports Level1 = org.nd4j.linalg.api.blas.Level1
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

Namespace org.deeplearning4j.graph.models.embeddings

	Public Class InMemoryGraphLookupTable
		Implements GraphVectorLookupTable

		Protected Friend nVertices As Integer
'JAVA TO VB CONVERTER NOTE: The field vectorSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vectorSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field tree was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend tree_Conflict As BinaryTree
'JAVA TO VB CONVERTER NOTE: The field vertexVectors was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vertexVectors_Conflict As INDArray ''input' vectors
'JAVA TO VB CONVERTER NOTE: The field outWeights was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend outWeights_Conflict As INDArray ''output' vectors. Specifically vectors for inner nodes in binary tree
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend learningRate_Conflict As Double

		Protected Friend expTable() As Double
		Protected Friend Shared MAX_EXP As Double = 6

		Public Sub New(ByVal nVertices As Integer, ByVal vectorSize As Integer, ByVal tree As BinaryTree, ByVal learningRate As Double)
			Me.nVertices = nVertices
			Me.vectorSize_Conflict = vectorSize
			Me.tree_Conflict = tree
			Me.learningRate_Conflict = learningRate
			resetWeights()

			expTable = New Double(999){}
			For i As Integer = 0 To expTable.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable.Length) * 2 - 1) * MAX_EXP)
				expTable(i) = tmp / (tmp + 1.0)
			Next i
		End Sub

		Public Overridable Property VertexVectors As INDArray
			Get
				Return vertexVectors_Conflict
			End Get
			Set(ByVal vertexVectors As INDArray)
				Me.vertexVectors_Conflict = vertexVectors
			End Set
		End Property

		Public Overridable ReadOnly Property OutWeights As INDArray
			Get
				Return outWeights_Conflict
			End Get
		End Property

		Public Overridable Function vectorSize() As Integer Implements GraphVectorLookupTable.vectorSize
			Return vectorSize_Conflict
		End Function

		Public Overridable Sub resetWeights() Implements GraphVectorLookupTable.resetWeights
			Me.vertexVectors_Conflict = Nd4j.rand(nVertices, vectorSize_Conflict).subi(0.5).divi(vectorSize_Conflict)
			Me.outWeights_Conflict = Nd4j.rand(nVertices - 1, vectorSize_Conflict).subi(0.5).divi(vectorSize_Conflict) 'Full binary tree with L leaves has L-1 inner nodes
		End Sub

		Public Overridable Sub iterate(ByVal first As Integer, ByVal second As Integer) Implements GraphVectorLookupTable.iterate
			'Get vectors and gradients
			'vecAndGrads[0][0] is vector of vertex(first); vecAndGrads[1][0] is corresponding gradient
			Dim vecAndGrads()() As INDArray = vectorsAndGradients(first, second)

			Dim l1 As Level1 = Nd4j.BlasWrapper.level1()
			Dim i As Integer = 0
			Do While i < vecAndGrads(0).Length
				'Update: v = v - lr * gradient
				l1.axpy(vecAndGrads(0)(i).length(), -learningRate_Conflict, vecAndGrads(1)(i), vecAndGrads(0)(i))
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Returns vertex vector and vector gradients, plus inner node vectors and inner node gradients<br>
		''' Specifically, out[0] are vectors, out[1] are gradients for the corresponding vectors<br>
		''' out[0][0] is vector for first vertex; out[0][1] is gradient for this vertex vector<br>
		''' out[0][i] (i>0) is the inner node vector along path to second vertex; out[1][i] is gradient for inner node vertex<br>
		''' This design is used primarily to aid in testing (numerical gradient checks) </summary>
		''' <param name="first"> first (input) vertex index </param>
		''' <param name="second"> second (output) vertex index </param>
		Public Overridable Function vectorsAndGradients(ByVal first As Integer, ByVal second As Integer) As INDArray()()
			'Input vertex vector gradients are composed of the inner node gradients
			'Get vector for first vertex, as well as code for second:
			Dim vec As INDArray = vertexVectors_Conflict.getRow(first)
			Dim codeLength As Integer = tree_Conflict.getCodeLength(second)
			Dim code As Long = tree_Conflict.getCode(second)
			Dim innerNodesForVertex() As Integer = tree_Conflict.getPathInnerNodes(second)

			Dim [out]()() As INDArray = { New INDArray(innerNodesForVertex.Length){}, New INDArray(innerNodesForVertex.Length){} }

			Dim l1 As Level1 = Nd4j.BlasWrapper.level1()
			Dim accumError As INDArray = Nd4j.create(vec.shape())
			For i As Integer = 0 To codeLength - 1

				'Inner node:
				Dim innerNodeIdx As Integer = innerNodesForVertex(i)
				Dim path As Boolean = getBit(code, i) 'left or right?

				Dim innerNodeVector As INDArray = outWeights_Conflict.getRow(innerNodeIdx)
				Dim sigmoidDot As Double = sigmoid(Nd4j.BlasWrapper.dot(innerNodeVector, vec))



				'Calculate gradient for inner node + accumulate error:
				Dim innerNodeGrad As INDArray
				If path Then
					innerNodeGrad = vec.mul(sigmoidDot - 1)
					l1.axpy(vec.length(), sigmoidDot - 1, innerNodeVector, accumError)
				Else
					innerNodeGrad = vec.mul(sigmoidDot)
					l1.axpy(vec.length(), sigmoidDot, innerNodeVector, accumError)
				End If

				[out](0)(i + 1) = innerNodeVector
				[out](1)(i + 1) = innerNodeGrad
			Next i

			[out](0)(0) = vec
			[out](1)(0) = accumError

			Return [out]
		End Function

		''' <summary>
		''' Calculate the probability of the second vertex given the first vertex
		''' i.e., P(v_second | v_first) </summary>
		''' <param name="first"> index of the first vertex </param>
		''' <param name="second"> index of the second vertex </param>
		''' <returns> probability, P(v_second | v_first) </returns>
		Public Overridable Function calculateProb(ByVal first As Integer, ByVal second As Integer) As Double
			'Get vector for first vertex, as well as code for second:
			Dim vec As INDArray = vertexVectors_Conflict.getRow(first)
			Dim codeLength As Integer = tree_Conflict.getCodeLength(second)
			Dim code As Long = tree_Conflict.getCode(second)
			Dim innerNodesForVertex() As Integer = tree_Conflict.getPathInnerNodes(second)

			Dim prob As Double = 1.0
			For i As Integer = 0 To codeLength - 1
				Dim path As Boolean = getBit(code, i) 'left or right?
				'Inner node:
				Dim innerNodeIdx As Integer = innerNodesForVertex(i)
				Dim nwi As INDArray = outWeights_Conflict.getRow(innerNodeIdx)

				Dim dot As Double = Nd4j.BlasWrapper.dot(nwi, vec)

				'double sigmoidDot = sigmoid(dot);
				Dim innerProb As Double = (If(path, sigmoid(dot), sigmoid(-dot))) 'prob of going left or right at inner node
				prob *= innerProb
			Next i
			Return prob
		End Function

		''' <summary>
		''' Calculate score. -log P(v_second | v_first) </summary>
		Public Overridable Function calculateScore(ByVal first As Integer, ByVal second As Integer) As Double
			'Score is -log P(out|in)
			Dim prob As Double = calculateProb(first, second)
			Return -FastMath.log(prob)
		End Function

		Public Overridable ReadOnly Property Tree As BinaryTree
			Get
				Return tree_Conflict
			End Get
		End Property

		Public Overridable Function getInnerNodeVector(ByVal innerNode As Integer) As INDArray
			Return outWeights_Conflict.getRow(innerNode)
		End Function

		Public Overridable Function getVector(ByVal idx As Integer) As INDArray Implements GraphVectorLookupTable.getVector
			Return vertexVectors_Conflict.getRow(idx)
		End Function

		Public Overridable WriteOnly Property LearningRate Implements GraphVectorLookupTable.setLearningRate As Double
			Set(ByVal learningRate As Double)
				Me.learningRate_Conflict = learningRate
			End Set
		End Property

		Public Overridable ReadOnly Property NumVertices As Integer Implements GraphVectorLookupTable.getNumVertices
			Get
				Return nVertices
			End Get
		End Property

		Private Shared Function sigmoid(ByVal [in] As Double) As Double
			Return 1.0 / (1.0 + FastMath.exp(-[in]))
		End Function

		Private Function getBit(ByVal [in] As Long, ByVal bitNum As Integer) As Boolean
			Dim mask As Long = 1L << bitNum
			Return ([in] And mask) <> 0L
		End Function

	End Class

End Namespace