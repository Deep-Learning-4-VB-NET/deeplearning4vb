Imports System
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports org.deeplearning4j.nn.conf.layers
Imports Activation = org.nd4j.linalg.activations.Activation

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

Namespace org.deeplearning4j.zoo.model.helper

	Public Class FaceNetHelper

		Public Shared ReadOnly Property ModuleName As String
			Get
				Return "inception"
			End Get
		End Property

		Public Shared Function getModuleName(ByVal layerName As String) As String
			Return ModuleName & "-" & layerName
		End Function


		Public Shared Function conv1x1(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1}, New Integer() {0, 0})).nIn([in]).nOut([out]).biasInit(bias).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build()
		End Function

		Public Shared Function c3x3reduce(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return conv1x1([in], [out], bias)
		End Function

		Public Shared Function c5x5reduce(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return conv1x1([in], [out], bias)
		End Function

		Public Shared Function conv3x3(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {1, 1}, New Integer() {1, 1})).nIn([in]).nOut([out]).biasInit(bias).build()
		End Function

		Public Shared Function conv5x5(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() {5, 5}, New Integer() {1, 1}, New Integer() {2, 2})).nIn([in]).nOut([out]).biasInit(bias).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build()
		End Function

		Public Shared Function conv7x7(ByVal [in] As Integer, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() {7, 7}, New Integer() {2, 2}, New Integer() {3, 3})).nIn([in]).nOut([out]).biasInit(bias).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build()
		End Function

		Public Shared Function avgPool7x7(ByVal stride As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {7, 7}, New Integer() {1, 1})).build()
		End Function

		Public Shared Function avgPoolNxN(ByVal size As Integer, ByVal stride As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {size, size}, New Integer() {stride, stride})).build()
		End Function

		Public Shared Function pNormNxN(ByVal pNorm As Integer, ByVal size As Integer, ByVal stride As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.PNORM, New Integer() {size, size}, New Integer() {stride, stride})).pnorm(pNorm).build()
		End Function

		Public Shared Function maxPool3x3(ByVal stride As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(New Integer() {3, 3}, New Integer() {stride, stride}, New Integer() {1, 1})).build()
		End Function

		Public Shared Function maxPoolNxN(ByVal size As Integer, ByVal stride As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(New Integer() {size, size}, New Integer() {stride, stride}, New Integer() {1, 1})).build()
		End Function

		Public Shared Function fullyConnected(ByVal [in] As Integer, ByVal [out] As Integer, ByVal dropOut As Double) As DenseLayer
			Return (New DenseLayer.Builder()).nIn([in]).nOut([out]).dropOut(dropOut).build()
		End Function

		Public Shared Function convNxN(ByVal reduceSize As Integer, ByVal outputSize As Integer, ByVal kernelSize As Integer, ByVal kernelStride As Integer, ByVal padding As Boolean) As ConvolutionLayer
			Dim pad As Integer = If(padding, (CInt(Math.Floor(kernelStride \ 2)) * 2), 0)
			Return (New ConvolutionLayer.Builder(New Integer() {kernelSize, kernelSize}, New Integer() {kernelStride, kernelStride}, New Integer() {pad, pad})).nIn(reduceSize).nOut(outputSize).biasInit(0.2).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build()
		End Function

		Public Shared Function convNxNreduce(ByVal inputSize As Integer, ByVal reduceSize As Integer, ByVal reduceStride As Integer) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {reduceStride, reduceStride})).nIn(inputSize).nOut(reduceSize).biasInit(0.2).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build()
		End Function

		Public Shared Function batchNorm(ByVal [in] As Integer, ByVal [out] As Integer) As BatchNormalization
			Return (New BatchNormalization.Builder(False)).nIn([in]).nOut([out]).build()
		End Function

		Public Shared Function appendGraph(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal moduleLayerName As String, ByVal inputSize As Integer, ByVal kernelSize() As Integer, ByVal kernelStride() As Integer, ByVal outputSize() As Integer, ByVal reduceSize() As Integer, ByVal poolingType As SubsamplingLayer.PoolingType, ByVal transferFunction As Activation, ByVal inputLayer As String) As ComputationGraphConfiguration.GraphBuilder
			Return appendGraph(graph, moduleLayerName, inputSize, kernelSize, kernelStride, outputSize, reduceSize, poolingType, 0, 3, 1, transferFunction, inputLayer)
		End Function

		Public Shared Function appendGraph(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal moduleLayerName As String, ByVal inputSize As Integer, ByVal kernelSize() As Integer, ByVal kernelStride() As Integer, ByVal outputSize() As Integer, ByVal reduceSize() As Integer, ByVal poolingType As SubsamplingLayer.PoolingType, ByVal pNorm As Integer, ByVal transferFunction As Activation, ByVal inputLayer As String) As ComputationGraphConfiguration.GraphBuilder
			Return appendGraph(graph, moduleLayerName, inputSize, kernelSize, kernelStride, outputSize, reduceSize, poolingType, pNorm, 3, 1, transferFunction, inputLayer)
		End Function

		Public Shared Function appendGraph(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal moduleLayerName As String, ByVal inputSize As Integer, ByVal kernelSize() As Integer, ByVal kernelStride() As Integer, ByVal outputSize() As Integer, ByVal reduceSize() As Integer, ByVal poolingType As SubsamplingLayer.PoolingType, ByVal poolSize As Integer, ByVal poolStride As Integer, ByVal transferFunction As Activation, ByVal inputLayer As String) As ComputationGraphConfiguration.GraphBuilder
			Return appendGraph(graph, moduleLayerName, inputSize, kernelSize, kernelStride, outputSize, reduceSize, poolingType, 0, poolSize, poolStride, transferFunction, inputLayer)
		End Function

		''' <summary>
		''' Appends inception layer configurations a GraphBuilder object, based on the concept of
		''' Inception via the GoogleLeNet paper: https://arxiv.org/abs/1409.4842
		''' </summary>
		''' <param name="graph"> An existing computation graph GraphBuilder object. </param>
		''' <param name="moduleLayerName"> The numerical order of inception (like 2, 2a, 3e, etc.) </param>
		''' <param name="inputSize"> </param>
		''' <param name="kernelSize"> </param>
		''' <param name="kernelStride"> </param>
		''' <param name="outputSize"> </param>
		''' <param name="reduceSize"> </param>
		''' <param name="poolingType"> </param>
		''' <param name="poolSize"> </param>
		''' <param name="poolStride"> </param>
		''' <param name="inputLayer">
		''' @return </param>
		Public Shared Function appendGraph(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal moduleLayerName As String, ByVal inputSize As Integer, ByVal kernelSize() As Integer, ByVal kernelStride() As Integer, ByVal outputSize() As Integer, ByVal reduceSize() As Integer, ByVal poolingType As SubsamplingLayer.PoolingType, ByVal pNorm As Integer, ByVal poolSize As Integer, ByVal poolStride As Integer, ByVal transferFunction As Activation, ByVal inputLayer As String) As ComputationGraphConfiguration.GraphBuilder
			' 1x1 reduce -> nxn conv
			For i As Integer = 0 To kernelSize.Length - 1
				graph.addLayer(getModuleName(moduleLayerName) & "-cnn1-" & i, conv1x1(inputSize, reduceSize(i), 0.2), inputLayer)
				graph.addLayer(getModuleName(moduleLayerName) & "-batch1-" & i, batchNorm(reduceSize(i), reduceSize(i)), getModuleName(moduleLayerName) & "-cnn1-" & i)
				graph.addLayer(getModuleName(moduleLayerName) & "-transfer1-" & i, (New ActivationLayer.Builder()).activation(transferFunction).build(), getModuleName(moduleLayerName) & "-batch1-" & i)
				graph.addLayer(getModuleName(moduleLayerName) & "-reduce1-" & i, convNxN(reduceSize(i), outputSize(i), kernelSize(i), kernelStride(i), True), getModuleName(moduleLayerName) & "-transfer1-" & i)
				graph.addLayer(getModuleName(moduleLayerName) & "-batch2-" & i, batchNorm(outputSize(i), outputSize(i)), getModuleName(moduleLayerName) & "-reduce1-" & i)
				graph.addLayer(getModuleName(moduleLayerName) & "-transfer2-" & i, (New ActivationLayer.Builder()).activation(transferFunction).build(), getModuleName(moduleLayerName) & "-batch2-" & i)
			Next i

			' pool -> 1x1 conv
			Dim i As Integer = kernelSize.Length
			Try
				Dim checkIndex As Integer = reduceSize(i)
				Select Case poolingType.innerEnumValue
					Case org.deeplearning4j.nn.conf.layers.SubsamplingLayer.PoolingType.InnerEnum.AVG
						graph.addLayer(getModuleName(moduleLayerName) & "-pool1", avgPoolNxN(poolSize, poolStride), inputLayer)
					Case org.deeplearning4j.nn.conf.layers.SubsamplingLayer.PoolingType.InnerEnum.MAX
						graph.addLayer(getModuleName(moduleLayerName) & "-pool1", maxPoolNxN(poolSize, poolStride), inputLayer)
					Case org.deeplearning4j.nn.conf.layers.SubsamplingLayer.PoolingType.InnerEnum.PNORM
						If pNorm <= 0 Then
							Throw New System.ArgumentException("p-norm must be greater than zero.")
						End If
						graph.addLayer(getModuleName(moduleLayerName) & "-pool1", pNormNxN(pNorm, poolSize, poolStride), inputLayer)
					Case Else
						Throw New System.InvalidOperationException("You must specify a valid pooling type of avg or max for Inception module.")
				End Select
				graph.addLayer(getModuleName(moduleLayerName) & "-cnn2", convNxNreduce(inputSize, reduceSize(i), 1), getModuleName(moduleLayerName) & "-pool1")
				graph.addLayer(getModuleName(moduleLayerName) & "-batch3", batchNorm(reduceSize(i), reduceSize(i)), getModuleName(moduleLayerName) & "-cnn2")
				graph.addLayer(getModuleName(moduleLayerName) & "-transfer3", (New ActivationLayer.Builder()).activation(transferFunction).build(), getModuleName(moduleLayerName) & "-batch3")
			Catch e As System.IndexOutOfRangeException
			End Try
			i += 1

			' reduce
			Try
				graph.addLayer(getModuleName(moduleLayerName) & "-reduce2", convNxNreduce(inputSize, reduceSize(i), 1), inputLayer)
				graph.addLayer(getModuleName(moduleLayerName) & "-batch4", batchNorm(reduceSize(i), reduceSize(i)), getModuleName(moduleLayerName) & "-reduce2")
				graph.addLayer(getModuleName(moduleLayerName) & "-transfer4", (New ActivationLayer.Builder()).activation(transferFunction).build(), getModuleName(moduleLayerName) & "-batch4")
			Catch e As System.IndexOutOfRangeException
			End Try

			' TODO: there's a better way to do this
			If kernelSize.Length = 1 AndAlso reduceSize.Length = 3 Then
				graph.addVertex(getModuleName(moduleLayerName), New MergeVertex(), getModuleName(moduleLayerName) & "-transfer2-0", getModuleName(moduleLayerName) & "-transfer3", getModuleName(moduleLayerName) & "-transfer4")
			ElseIf kernelSize.Length = 2 AndAlso reduceSize.Length = 2 Then
				graph.addVertex(getModuleName(moduleLayerName), New MergeVertex(), getModuleName(moduleLayerName) & "-transfer2-0", getModuleName(moduleLayerName) & "-transfer2-1")
			ElseIf kernelSize.Length = 2 AndAlso reduceSize.Length = 3 Then
				graph.addVertex(getModuleName(moduleLayerName), New MergeVertex(), getModuleName(moduleLayerName) & "-transfer2-0", getModuleName(moduleLayerName) & "-transfer2-1", getModuleName(moduleLayerName) & "-transfer3")
			ElseIf kernelSize.Length = 2 AndAlso reduceSize.Length = 4 Then
				graph.addVertex(getModuleName(moduleLayerName), New MergeVertex(), getModuleName(moduleLayerName) & "-transfer2-0", getModuleName(moduleLayerName) & "-transfer2-1", getModuleName(moduleLayerName) & "-transfer3", getModuleName(moduleLayerName) & "-transfer4")
			Else
				Throw New System.InvalidOperationException("Only kernel of shape 1 or 2 and a reduce shape between 2 and 4 is supported.")
			End If

			Return graph
		End Function

	End Class

End Namespace