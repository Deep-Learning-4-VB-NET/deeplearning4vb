Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.convolution


	<Serializable>
	Public Class Cnn3DLossLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.Cnn3DLossLayer)
		Implements IOutputLayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected org.nd4j.linalg.api.ndarray.INDArray labels;
		Protected Friend labels As INDArray

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If input_Conflict.rank() <> 5 Then
				Throw New System.NotSupportedException("Input is not rank 5. Got input with rank " & input_Conflict.rank() & " " & layerId() & " with shape " & Arrays.toString(input_Conflict.shape()) & " - expected shape [minibatch,channels,depth,height,width]")
			End If
			If labels Is Nothing Then
				Throw New System.InvalidOperationException("Labels are not set (null)")
			End If

			Dim input2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim labels2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), labels, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim maskReshaped As INDArray = ConvolutionUtils.reshapeCnn3dMask(layerConf().getDataFormat(), maskArray_Conflict, labels, workspaceMgr, ArrayType.FF_WORKING_MEM)

			' delta calculation
			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim delta2d As INDArray = lossFunction.computeGradient(labels2d, input2d.dup(input2d.ordering()), layerConf().getActivationFn(), maskReshaped)
			delta2d = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, delta2d)

			Dim n As Long = input_Conflict.size(0)
			Dim d, h, w, c As Long
			If layerConf().getDataFormat() = Convolution3D.DataFormat.NDHWC Then
				d = input_Conflict.size(1)
				h = input_Conflict.size(2)
				w = input_Conflict.size(3)
				c = input_Conflict.size(4)
			Else
				d = input_Conflict.size(2)
				h = input_Conflict.size(3)
				w = input_Conflict.size(4)
				c = input_Conflict.size(1)
			End If
			Dim delta5d As INDArray = ConvolutionUtils.reshape2dTo5d(layerConf().getDataFormat(), delta2d, n, d, h, w, c, workspaceMgr, ArrayType.ACTIVATION_GRAD)

			' grab the empty gradient
			Dim gradient As Gradient = New DefaultGradient()
			Return New Pair(Of Gradient, INDArray)(gradient, delta5d)
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overridable Function f1Score(ByVal data As DataSet) As Double
			Return 0
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Dim [out] As INDArray = activate(examples, False, Nothing) 'TODO
			Dim eval As New Evaluation()
			eval.evalTimeSeries(labels, [out], maskArray_Conflict)
			Return eval.f1()
		End Function

		Public Overridable Function numLabels() As Integer
			Return CInt(labels.size(1))
		End Function

		Public Overridable Overloads Sub fit(ByVal iter As DataSetIterator)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function predict(ByVal examples As INDArray) As Integer()
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function predict(ByVal dataSet As DataSet) As IList(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function type() As Type
			Return Type.CONVOLUTIONAL3D
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If input_Conflict.rank() <> 5 Then
				Throw New System.NotSupportedException("Input must be rank 5. Got input with rank " & input_Conflict.rank() & " " & layerId())
			End If

			Dim [in] As INDArray = workspaceMgr.dup(ArrayType.ACTIVATIONS, input_Conflict, input_Conflict.ordering())
			Dim input2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), [in], workspaceMgr, ArrayType.ACTIVATIONS)
			Dim out2d As INDArray = layerConf().getActivationFn().getActivation(input2d, training)

			Dim n As Long = input_Conflict.size(0)
			Dim d, h, w, c As Long
			If layerConf().getDataFormat() = Convolution3D.DataFormat.NDHWC Then
				d = CInt(input_Conflict.size(1))
				h = CInt(input_Conflict.size(2))
				w = CInt(input_Conflict.size(3))
				c = CInt(input_Conflict.size(4))
			Else
				d = CInt(input_Conflict.size(2))
				h = CInt(input_Conflict.size(3))
				w = CInt(input_Conflict.size(4))
				c = CInt(input_Conflict.size(1))
			End If

			Return ConvolutionUtils.reshape2dTo5d(layerConf().getDataFormat(), out2d, n, d, h, w, c, workspaceMgr, ArrayType.ACTIVATIONS)
		End Function

		Public Overrides WriteOnly Property MaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				Me.maskArray_Conflict = maskArray
			End Set
		End Property

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Me.maskArray_Conflict = maskArray
			Return Nothing 'Last layer in network
		End Function

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return True
		End Function

		Public Overridable Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements IOutputLayer.computeScore
			Dim input2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim labels2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), labels, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim maskReshaped As INDArray = ConvolutionUtils.reshapeCnn3dMask(layerConf().getDataFormat(), maskArray_Conflict, input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()

			Dim score As Double = lossFunction.computeScore(labels2d, input2d.dup(), layerConf().getActivationFn(), maskReshaped, False)
			score /= InputMiniBatchSize
			score += fullNetRegTerm
			Me.score_Conflict = score
			Return score
		End Function

		''' <summary>
		''' Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overridable Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IOutputLayer.computeScoreForExamples
			'For 3D CNN: need to sum up the score over each x/y/z location before returning

			If input_Conflict Is Nothing OrElse labels Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If

			Dim input2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim labels2d As INDArray = ConvolutionUtils.reshape5dTo2d(layerConf().getDataFormat(), labels, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim maskReshaped As INDArray = ConvolutionUtils.reshapeCnn3dMask(layerConf().getDataFormat(), maskArray_Conflict, input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(labels2d, input2d, layerConf().getActivationFn(), maskReshaped)
			'scoreArray: shape [minibatch*d*h*w, 1]
			'Reshape it to [minibatch, 1, d, h, w] then sum over x/y/z to give [minibatch, 1]

			Dim newShape As val = CType(input_Conflict.shape().Clone(), Long())
			newShape(1) = 1

			Dim n As Long = input_Conflict.size(0)
			Dim d, h, w, c As Long
			If layerConf().getDataFormat() = Convolution3D.DataFormat.NDHWC Then
				d = input_Conflict.size(1)
				h = input_Conflict.size(2)
				w = input_Conflict.size(3)
				c = input_Conflict.size(4)
			Else
				d = input_Conflict.size(2)
				h = input_Conflict.size(3)
				w = input_Conflict.size(4)
				c = input_Conflict.size(1)
			End If
			Dim scoreArrayTs As INDArray = ConvolutionUtils.reshape2dTo5d(layerConf().getDataFormat(), scoreArray, n, d, h, w, c, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim summedScores As INDArray = scoreArrayTs.sum(1,2,3,4)

			If fullNetRegTerm <> 0.0 Then
				summedScores.addi(fullNetRegTerm)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, summedScores)
		End Function
	End Class

End Namespace