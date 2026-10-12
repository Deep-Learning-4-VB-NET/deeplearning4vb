import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataFormat = org.nd4j.enums.DataFormat
Imports Conv1DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Conv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
Imports DeConv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig
Imports DeConv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports LocalResponseNormalizationConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig

''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.autodiff.samediff.ops

	Public Class SDCNN
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' 2D Convolution layer operation - average pooling 2d<br>
	  ''' </summary>
	  ''' <param name="input"> the input to average pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying average pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function avgPooling2d(ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable
		SDValidation.validateNumerical("avgPooling2d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling2D(sd,input, Pooling2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - average pooling 2d<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to average pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying average pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function avgPooling2d(ByVal name As String, ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable
		SDValidation.validateNumerical("avgPooling2d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling2D(sd,input, Pooling2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 3D convolution layer operation - average pooling 3d <br>
	  ''' </summary>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling3DConfig"> Configuration Object </param>
	  ''' <returns> output after applying average pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function avgPooling3d(ByVal input As SDVariable, ByVal Pooling3DConfig As Pooling3DConfig) As SDVariable
		SDValidation.validateNumerical("avgPooling3d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling3D(sd,input, Pooling3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 3D convolution layer operation - average pooling 3d <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling3DConfig"> Configuration Object </param>
	  ''' <returns> output after applying average pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function avgPooling3d(ByVal name As String, ByVal input As SDVariable, ByVal Pooling3DConfig As Pooling3DConfig) As SDVariable
		SDValidation.validateNumerical("avgPooling3d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling3D(sd,input, Pooling3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer batch to space operation on 4d input.<br>
	  ''' Reduces input batch dimension by rearranging data into a larger spatial dimensions<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable. 4d input (NUMERIC type) </param>
	  ''' <param name="blocks"> Block size, in the height/width dimension (Size: Exactly(count=2)) </param>
	  ''' <param name="croppingTop">  (Size: Exactly(count=2)) </param>
	  ''' <param name="croppingBottom">  (Size: Exactly(count=2)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function batchToSpace(ByVal x As SDVariable, ByVal blocks() As Integer, ByVal croppingTop() As Integer, ParamArray ByVal croppingBottom() As Integer) As SDVariable
		SDValidation.validateNumerical("batchToSpace", "x", x)
		Preconditions.checkArgument(blocks.Length = 2, "blocks has incorrect size/length. Expected: blocks.length == 2, got %s", blocks.Length)
		Preconditions.checkArgument(croppingTop.Length = 2, "croppingTop has incorrect size/length. Expected: croppingTop.length == 2, got %s", croppingTop.Length)
		Preconditions.checkArgument(croppingBottom.Length = 2, "croppingBottom has incorrect size/length. Expected: croppingBottom.length == 2, got %s", croppingBottom.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.BatchToSpace(sd,x, blocks, croppingTop, croppingBottom)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer batch to space operation on 4d input.<br>
	  ''' Reduces input batch dimension by rearranging data into a larger spatial dimensions<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable. 4d input (NUMERIC type) </param>
	  ''' <param name="blocks"> Block size, in the height/width dimension (Size: Exactly(count=2)) </param>
	  ''' <param name="croppingTop">  (Size: Exactly(count=2)) </param>
	  ''' <param name="croppingBottom">  (Size: Exactly(count=2)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function batchToSpace(ByVal name As String, ByVal x As SDVariable, ByVal blocks() As Integer, ByVal croppingTop() As Integer, ParamArray ByVal croppingBottom() As Integer) As SDVariable
		SDValidation.validateNumerical("batchToSpace", "x", x)
		Preconditions.checkArgument(blocks.Length = 2, "blocks has incorrect size/length. Expected: blocks.length == 2, got %s", blocks.Length)
		Preconditions.checkArgument(croppingTop.Length = 2, "croppingTop has incorrect size/length. Expected: croppingTop.length == 2, got %s", croppingTop.Length)
		Preconditions.checkArgument(croppingBottom.Length = 2, "croppingBottom has incorrect size/length. Expected: croppingBottom.length == 2, got %s", croppingBottom.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.BatchToSpace(sd,x, blocks, croppingTop, croppingBottom)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' col2im operation for use in 2D convolution operations. Outputs a 4d array with shape<br>
	  ''' [minibatch, inputChannels, height, width]<br>
	  ''' </summary>
	  ''' <param name="in"> Input - rank 6 input with shape [minibatch, inputChannels, kernelHeight, kernelWidth, outputHeight, outputWidth] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output Col2Im output variable (NUMERIC type) </returns>
	  Public Overridable Function col2Im(ByVal [in] As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("col2Im", "in", [in])
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Col2Im(sd,[in], Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' col2im operation for use in 2D convolution operations. Outputs a 4d array with shape<br>
	  ''' [minibatch, inputChannels, height, width]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input - rank 6 input with shape [minibatch, inputChannels, kernelHeight, kernelWidth, outputHeight, outputWidth] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output Col2Im output variable (NUMERIC type) </returns>
	  Public Overridable Function col2Im(ByVal name As String, ByVal [in] As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("col2Im", "in", [in])
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Col2Im(sd,[in], Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Conv1d operation.<br>
	  ''' </summary>
	  ''' <param name="input"> the inputs to conv1d (NUMERIC type) </param>
	  ''' <param name="weights"> weights for conv1d op - rank 3 array with shape [kernelSize, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="bias"> bias for conv1d op - rank 1 array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv1DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv1d op (NUMERIC type) </returns>
	  Public Overridable Function conv1d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv1DConfig As Conv1DConfig) As SDVariable
		SDValidation.validateNumerical("conv1d", "input", input)
		SDValidation.validateNumerical("conv1d", "weights", weights)
		SDValidation.validateNumerical("conv1d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D(sd,input, weights, bias, Conv1DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Conv1d operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the inputs to conv1d (NUMERIC type) </param>
	  ''' <param name="weights"> weights for conv1d op - rank 3 array with shape [kernelSize, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="bias"> bias for conv1d op - rank 1 array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv1DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv1d op (NUMERIC type) </returns>
	  Public Overridable Function conv1d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv1DConfig As Conv1DConfig) As SDVariable
		SDValidation.validateNumerical("conv1d", "input", input)
		SDValidation.validateNumerical("conv1d", "weights", weights)
		SDValidation.validateNumerical("conv1d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D(sd,input, weights, bias, Conv1DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Conv1d operation.<br>
	  ''' </summary>
	  ''' <param name="input"> the inputs to conv1d (NUMERIC type) </param>
	  ''' <param name="weights"> weights for conv1d op - rank 3 array with shape [kernelSize, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="Conv1DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv1d op (NUMERIC type) </returns>
	  Public Overridable Function conv1d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal Conv1DConfig As Conv1DConfig) As SDVariable
		SDValidation.validateNumerical("conv1d", "input", input)
		SDValidation.validateNumerical("conv1d", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D(sd,input, weights, Nothing, Conv1DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Conv1d operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the inputs to conv1d (NUMERIC type) </param>
	  ''' <param name="weights"> weights for conv1d op - rank 3 array with shape [kernelSize, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="Conv1DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv1d op (NUMERIC type) </returns>
	  Public Overridable Function conv1d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal Conv1DConfig As Conv1DConfig) As SDVariable
		SDValidation.validateNumerical("conv1d", "input", input)
		SDValidation.validateNumerical("conv1d", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D(sd,input, weights, Nothing, Conv1DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D Convolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the convolution operation. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv2d op (NUMERIC type) </returns>
	  Public Overridable Function conv2d(ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("conv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("conv2d", "weights", weights)
		SDValidation.validateNumerical("conv2d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D(sd,layerInput, weights, bias, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the convolution operation. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv2d op (NUMERIC type) </returns>
	  Public Overridable Function conv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("conv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("conv2d", "weights", weights)
		SDValidation.validateNumerical("conv2d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D(sd,layerInput, weights, bias, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D Convolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the convolution operation. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv2d op (NUMERIC type) </returns>
	  Public Overridable Function conv2d(ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("conv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("conv2d", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D(sd,layerInput, weights, Nothing, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the convolution operation. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, outputChannels] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of conv2d op (NUMERIC type) </returns>
	  Public Overridable Function conv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("conv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("conv2d", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D(sd,layerInput, weights, Nothing, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 3D operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights">  Weights for conv3d. Rank 5 with shape [kernelDepth, kernelHeight, kernelWidth, inputChannels, outputChannels]. (NUMERIC type) </param>
	  ''' <param name="bias">  Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv3DConfig"> Configuration Object </param>
	  ''' <returns> output Conv3d output variable (NUMERIC type) </returns>
	  Public Overridable Function conv3d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv3DConfig As Conv3DConfig) As SDVariable
		SDValidation.validateNumerical("conv3d", "input", input)
		SDValidation.validateNumerical("conv3d", "weights", weights)
		SDValidation.validateNumerical("conv3d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv3D(sd,input, weights, bias, Conv3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 3D operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights">  Weights for conv3d. Rank 5 with shape [kernelDepth, kernelHeight, kernelWidth, inputChannels, outputChannels]. (NUMERIC type) </param>
	  ''' <param name="bias">  Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv3DConfig"> Configuration Object </param>
	  ''' <returns> output Conv3d output variable (NUMERIC type) </returns>
	  Public Overridable Function conv3d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal Conv3DConfig As Conv3DConfig) As SDVariable
		SDValidation.validateNumerical("conv3d", "input", input)
		SDValidation.validateNumerical("conv3d", "weights", weights)
		SDValidation.validateNumerical("conv3d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv3D(sd,input, weights, bias, Conv3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 3D operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights">  Weights for conv3d. Rank 5 with shape [kernelDepth, kernelHeight, kernelWidth, inputChannels, outputChannels]. (NUMERIC type) </param>
	  ''' <param name="Conv3DConfig"> Configuration Object </param>
	  ''' <returns> output Conv3d output variable (NUMERIC type) </returns>
	  Public Overridable Function conv3d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal Conv3DConfig As Conv3DConfig) As SDVariable
		SDValidation.validateNumerical("conv3d", "input", input)
		SDValidation.validateNumerical("conv3d", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv3D(sd,input, weights, Nothing, Conv3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 3D operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights">  Weights for conv3d. Rank 5 with shape [kernelDepth, kernelHeight, kernelWidth, inputChannels, outputChannels]. (NUMERIC type) </param>
	  ''' <param name="Conv3DConfig"> Configuration Object </param>
	  ''' <returns> output Conv3d output variable (NUMERIC type) </returns>
	  Public Overridable Function conv3d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal Conv3DConfig As Conv3DConfig) As SDVariable
		SDValidation.validateNumerical("conv3d", "input", input)
		SDValidation.validateNumerical("conv3d", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Conv3D(sd,input, weights, Nothing, Conv3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D deconvolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to deconvolution 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the 2d deconvolution operation. 4 dimensions with format [inputChannels, outputChannels, kernelHeight, kernelWidth] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="DeConv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of deconv2d op (NUMERIC type) </returns>
	  Public Overridable Function deconv2d(ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal DeConv2DConfig As DeConv2DConfig) As SDVariable
		SDValidation.validateNumerical("deconv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("deconv2d", "weights", weights)
		SDValidation.validateNumerical("deconv2d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv2D(sd,layerInput, weights, bias, DeConv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D deconvolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to deconvolution 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the 2d deconvolution operation. 4 dimensions with format [inputChannels, outputChannels, kernelHeight, kernelWidth] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="DeConv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of deconv2d op (NUMERIC type) </returns>
	  Public Overridable Function deconv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal DeConv2DConfig As DeConv2DConfig) As SDVariable
		SDValidation.validateNumerical("deconv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("deconv2d", "weights", weights)
		SDValidation.validateNumerical("deconv2d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv2D(sd,layerInput, weights, bias, DeConv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D deconvolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to deconvolution 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the 2d deconvolution operation. 4 dimensions with format [inputChannels, outputChannels, kernelHeight, kernelWidth] (NUMERIC type) </param>
	  ''' <param name="DeConv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of deconv2d op (NUMERIC type) </returns>
	  Public Overridable Function deconv2d(ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal DeConv2DConfig As DeConv2DConfig) As SDVariable
		SDValidation.validateNumerical("deconv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("deconv2d", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv2D(sd,layerInput, weights, Nothing, DeConv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D deconvolution operation with optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to deconvolution 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights for the 2d deconvolution operation. 4 dimensions with format [inputChannels, outputChannels, kernelHeight, kernelWidth] (NUMERIC type) </param>
	  ''' <param name="DeConv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of deconv2d op (NUMERIC type) </returns>
	  Public Overridable Function deconv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal weights As SDVariable, ByVal DeConv2DConfig As DeConv2DConfig) As SDVariable
		SDValidation.validateNumerical("deconv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("deconv2d", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv2D(sd,layerInput, weights, Nothing, DeConv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 3D CNN deconvolution operation with or without optional bias<br>
	  ''' </summary>
	  ''' <param name="input"> Input array - shape [bS, iD, iH, iW, iC] (NDHWC) or [bS, iC, iD, iH, iW] (NCDHW) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array - shape [kD, kH, kW, oC, iC] (NUMERIC type) </param>
	  ''' <param name="bias"> Bias array - optional, may be null. If non-null, must have shape [outputChannels] (NUMERIC type) </param>
	  ''' <param name="DeConv3DConfig"> Configuration Object </param>
	  ''' <returns> output result of 3D CNN deconvolution operation (NUMERIC type) </returns>
	  Public Overridable Function deconv3d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal DeConv3DConfig As DeConv3DConfig) As SDVariable
		SDValidation.validateNumerical("deconv3d", "input", input)
		SDValidation.validateNumerical("deconv3d", "weights", weights)
		SDValidation.validateNumerical("deconv3d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3D(sd,input, weights, bias, DeConv3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 3D CNN deconvolution operation with or without optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input array - shape [bS, iD, iH, iW, iC] (NDHWC) or [bS, iC, iD, iH, iW] (NCDHW) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array - shape [kD, kH, kW, oC, iC] (NUMERIC type) </param>
	  ''' <param name="bias"> Bias array - optional, may be null. If non-null, must have shape [outputChannels] (NUMERIC type) </param>
	  ''' <param name="DeConv3DConfig"> Configuration Object </param>
	  ''' <returns> output result of 3D CNN deconvolution operation (NUMERIC type) </returns>
	  Public Overridable Function deconv3d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal DeConv3DConfig As DeConv3DConfig) As SDVariable
		SDValidation.validateNumerical("deconv3d", "input", input)
		SDValidation.validateNumerical("deconv3d", "weights", weights)
		SDValidation.validateNumerical("deconv3d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3D(sd,input, weights, bias, DeConv3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 3D CNN deconvolution operation with or without optional bias<br>
	  ''' </summary>
	  ''' <param name="input"> Input array - shape [bS, iD, iH, iW, iC] (NDHWC) or [bS, iC, iD, iH, iW] (NCDHW) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array - shape [kD, kH, kW, oC, iC] (NUMERIC type) </param>
	  ''' <param name="DeConv3DConfig"> Configuration Object </param>
	  ''' <returns> output result of 3D CNN deconvolution operation (NUMERIC type) </returns>
	  Public Overridable Function deconv3d(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal DeConv3DConfig As DeConv3DConfig) As SDVariable
		SDValidation.validateNumerical("deconv3d", "input", input)
		SDValidation.validateNumerical("deconv3d", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3D(sd,input, weights, Nothing, DeConv3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 3D CNN deconvolution operation with or without optional bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input array - shape [bS, iD, iH, iW, iC] (NDHWC) or [bS, iC, iD, iH, iW] (NCDHW) (NUMERIC type) </param>
	  ''' <param name="weights"> Weights array - shape [kD, kH, kW, oC, iC] (NUMERIC type) </param>
	  ''' <param name="DeConv3DConfig"> Configuration Object </param>
	  ''' <returns> output result of 3D CNN deconvolution operation (NUMERIC type) </returns>
	  Public Overridable Function deconv3d(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal DeConv3DConfig As DeConv3DConfig) As SDVariable
		SDValidation.validateNumerical("deconv3d", "input", input)
		SDValidation.validateNumerical("deconv3d", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3D(sd,input, weights, Nothing, DeConv3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer batch to space operation on 4d input.<br>
	  ''' Reduces input channels dimension by rearranging data into a larger spatial dimensions<br>
	  ''' Example: if input has shape [mb, 8, 2, 2] and block size is 2, then output size is [mb, 8/(2*2), 2*2, 2*2]<br>
	  ''' = [mb, 2, 4, 4]<br>
	  ''' </summary>
	  ''' <param name="x"> the input to depth to space pooling 2d operation - 4d activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="blockSize"> Block size, in the height/width dimension </param>
	  ''' <param name="dataFormat"> Data format: "NCHW" or "NHWC" </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function depthToSpace(ByVal x As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat) As SDVariable
		SDValidation.validateNumerical("depthToSpace", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthToSpace(sd,x, blockSize, dataFormat)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer batch to space operation on 4d input.<br>
	  ''' Reduces input channels dimension by rearranging data into a larger spatial dimensions<br>
	  ''' Example: if input has shape [mb, 8, 2, 2] and block size is 2, then output size is [mb, 8/(2*2), 2*2, 2*2]<br>
	  ''' = [mb, 2, 4, 4]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> the input to depth to space pooling 2d operation - 4d activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="blockSize"> Block size, in the height/width dimension </param>
	  ''' <param name="dataFormat"> Data format: "NCHW" or "NHWC" </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function depthToSpace(ByVal name As String, ByVal x As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat) As SDVariable
		SDValidation.validateNumerical("depthToSpace", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthToSpace(sd,x, blockSize, dataFormat)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Depth-wise 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Depth-wise conv2d weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of depthwise conv2d op (NUMERIC type) </returns>
	  Public Overridable Function depthWiseConv2d(ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("depthWiseConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("depthWiseConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("depthWiseConv2d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthwiseConv2D(sd,layerInput, depthWeights, bias, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Depth-wise 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Depth-wise conv2d weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional 1D bias array with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of depthwise conv2d op (NUMERIC type) </returns>
	  Public Overridable Function depthWiseConv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("depthWiseConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("depthWiseConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("depthWiseConv2d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthwiseConv2D(sd,layerInput, depthWeights, bias, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Depth-wise 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Depth-wise conv2d weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of depthwise conv2d op (NUMERIC type) </returns>
	  Public Overridable Function depthWiseConv2d(ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("depthWiseConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("depthWiseConv2d", "depthWeights", depthWeights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthwiseConv2D(sd,layerInput, depthWeights, Nothing, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Depth-wise 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Depth-wise conv2d weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of depthwise conv2d op (NUMERIC type) </returns>
	  Public Overridable Function depthWiseConv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("depthWiseConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("depthWiseConv2d", "depthWeights", depthWeights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.DepthwiseConv2D(sd,layerInput, depthWeights, Nothing, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' TODO doc string<br>
	  ''' </summary>
	  ''' <param name="df">  (NUMERIC type) </param>
	  ''' <param name="weights"> df (NUMERIC type) </param>
	  ''' <param name="strides"> weights (Size: Exactly(count=2)) </param>
	  ''' <param name="rates"> strides (Size: Exactly(count=2)) </param>
	  ''' <param name="isSameMode"> isSameMode </param>
	  ''' <returns> output Computed the grayscale dilation of 4-D input and 3-D filters tensors. (NUMERIC type) </returns>
	  Public Overridable Function dilation2D(ByVal df As SDVariable, ByVal weights As SDVariable, ByVal strides() As Integer, ByVal rates() As Integer, ByVal isSameMode As Boolean) As SDVariable
		SDValidation.validateNumerical("dilation2D", "df", df)
		SDValidation.validateNumerical("dilation2D", "weights", weights)
		Preconditions.checkArgument(strides.Length = 2, "strides has incorrect size/length. Expected: strides.length == 2, got %s", strides.Length)
		Preconditions.checkArgument(rates.Length = 2, "rates has incorrect size/length. Expected: rates.length == 2, got %s", rates.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Dilation2D(sd,df, weights, strides, rates, isSameMode)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' TODO doc string<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="df">  (NUMERIC type) </param>
	  ''' <param name="weights"> df (NUMERIC type) </param>
	  ''' <param name="strides"> weights (Size: Exactly(count=2)) </param>
	  ''' <param name="rates"> strides (Size: Exactly(count=2)) </param>
	  ''' <param name="isSameMode"> isSameMode </param>
	  ''' <returns> output Computed the grayscale dilation of 4-D input and 3-D filters tensors. (NUMERIC type) </returns>
	  Public Overridable Function dilation2D(ByVal name As String, ByVal df As SDVariable, ByVal weights As SDVariable, ByVal strides() As Integer, ByVal rates() As Integer, ByVal isSameMode As Boolean) As SDVariable
		SDValidation.validateNumerical("dilation2D", "df", df)
		SDValidation.validateNumerical("dilation2D", "weights", weights)
		Preconditions.checkArgument(strides.Length = 2, "strides has incorrect size/length. Expected: strides.length == 2, got %s", strides.Length)
		Preconditions.checkArgument(rates.Length = 2, "rates has incorrect size/length. Expected: rates.length == 2, got %s", rates.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Dilation2D(sd,df, weights, strides, rates, isSameMode)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Extract image patches <br>
	  ''' </summary>
	  ''' <param name="input"> Input array. Must be rank 4, with shape [minibatch, height, width, channels] (NUMERIC type) </param>
	  ''' <param name="kH"> Kernel height </param>
	  ''' <param name="kW"> Kernel width </param>
	  ''' <param name="sH"> Stride height </param>
	  ''' <param name="sW"> Stride width </param>
	  ''' <param name="rH"> Rate height </param>
	  ''' <param name="rW"> Rate width </param>
	  ''' <param name="sameMode"> If true: use same mode padding. If false </param>
	  ''' <returns> output The result is a 4D tensor which is indexed by batch, row, and column. (NUMERIC type) </returns>
	  Public Overridable Function extractImagePatches(ByVal input As SDVariable, ByVal kH As Integer, ByVal kW As Integer, ByVal sH As Integer, ByVal sW As Integer, ByVal rH As Integer, ByVal rW As Integer, ByVal sameMode As Boolean) As SDVariable
		SDValidation.validateNumerical("extractImagePatches", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.image.ExtractImagePatches(sd,input, kH, kW, sH, sW, rH, rW, sameMode)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Extract image patches <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input array. Must be rank 4, with shape [minibatch, height, width, channels] (NUMERIC type) </param>
	  ''' <param name="kH"> Kernel height </param>
	  ''' <param name="kW"> Kernel width </param>
	  ''' <param name="sH"> Stride height </param>
	  ''' <param name="sW"> Stride width </param>
	  ''' <param name="rH"> Rate height </param>
	  ''' <param name="rW"> Rate width </param>
	  ''' <param name="sameMode"> If true: use same mode padding. If false </param>
	  ''' <returns> output The result is a 4D tensor which is indexed by batch, row, and column. (NUMERIC type) </returns>
	  Public Overridable Function extractImagePatches(ByVal name As String, ByVal input As SDVariable, ByVal kH As Integer, ByVal kW As Integer, ByVal sH As Integer, ByVal sW As Integer, ByVal rH As Integer, ByVal rW As Integer, ByVal sameMode As Boolean) As SDVariable
		SDValidation.validateNumerical("extractImagePatches", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.image.ExtractImagePatches(sd,input, kH, kW, sH, sW, rH, rW, sameMode)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' im2col operation for use in 2D convolution operations. Outputs a 6d array with shape<br>
	  ''' [minibatch, inputChannels, kernelHeight, kernelWidth, outputHeight, outputWidth]   <br>
	  ''' </summary>
	  ''' <param name="in"> Input - rank 4 input with shape [minibatch, inputChannels, height, width] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output Im2Col output variable (NUMERIC type) </returns>
	  Public Overridable Function im2Col(ByVal [in] As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("im2Col", "in", [in])
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Im2col(sd,[in], Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' im2col operation for use in 2D convolution operations. Outputs a 6d array with shape<br>
	  ''' [minibatch, inputChannels, kernelHeight, kernelWidth, outputHeight, outputWidth]   <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input - rank 4 input with shape [minibatch, inputChannels, height, width] (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output Im2Col output variable (NUMERIC type) </returns>
	  Public Overridable Function im2Col(ByVal name As String, ByVal [in] As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("im2Col", "in", [in])
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Im2col(sd,[in], Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D convolution layer operation - local response normalization<br>
	  ''' </summary>
	  ''' <param name="input"> the inputs to lrn (NUMERIC type) </param>
	  ''' <param name="LocalResponseNormalizationConfig"> Configuration Object </param>
	  ''' <returns> output Result after Local Response Normalization (NUMERIC type) </returns>
	  Public Overridable Function localResponseNormalization(ByVal input As SDVariable, ByVal LocalResponseNormalizationConfig As LocalResponseNormalizationConfig) As SDVariable
		SDValidation.validateNumerical("localResponseNormalization", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.LocalResponseNormalization(sd,input, LocalResponseNormalizationConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D convolution layer operation - local response normalization<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the inputs to lrn (NUMERIC type) </param>
	  ''' <param name="LocalResponseNormalizationConfig"> Configuration Object </param>
	  ''' <returns> output Result after Local Response Normalization (NUMERIC type) </returns>
	  Public Overridable Function localResponseNormalization(ByVal name As String, ByVal input As SDVariable, ByVal LocalResponseNormalizationConfig As LocalResponseNormalizationConfig) As SDVariable
		SDValidation.validateNumerical("localResponseNormalization", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.LocalResponseNormalization(sd,input, LocalResponseNormalizationConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - Max pooling on the input and outputs both max values and indices <br>
	  ''' </summary>
	  ''' <param name="input"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  Public Overridable Function maxPoolWithArgmax(ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable()
		SDValidation.validateNumerical("maxPoolWithArgmax", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPoolWithArgmax(sd,input, Pooling2DConfig)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - Max pooling on the input and outputs both max values and indices <br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="input"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  Public Overridable Function maxPoolWithArgmax(ByVal names() As String, ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable()
		SDValidation.validateNumerical("maxPoolWithArgmax", "input", input)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPoolWithArgmax(sd,input, Pooling2DConfig)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - max pooling 2d <br>
	  ''' </summary>
	  ''' <param name="input"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying max pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function maxPooling2d(ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable
		SDValidation.validateNumerical("maxPooling2d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling2D(sd,input, Pooling2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - max pooling 2d <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling2DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying max pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function maxPooling2d(ByVal name As String, ByVal input As SDVariable, ByVal Pooling2DConfig As Pooling2DConfig) As SDVariable
		SDValidation.validateNumerical("maxPooling2d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling2D(sd,input, Pooling2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 3D convolution layer operation - max pooling 3d operation.<br>
	  ''' </summary>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling3DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying max pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function maxPooling3d(ByVal input As SDVariable, ByVal Pooling3DConfig As Pooling3DConfig) As SDVariable
		SDValidation.validateNumerical("maxPooling3d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling3D(sd,input, Pooling3DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 3D convolution layer operation - max pooling 3d operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> the input to average pooling 3d operation - 5d activations in NCDHW format (shape [minibatch, channels, depth, height, width]) or NDHWC format (shape [minibatch, depth, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="Pooling3DConfig"> Configuration Object </param>
	  ''' <returns> output Result after applying max pooling on the input (NUMERIC type) </returns>
	  Public Overridable Function maxPooling3d(ByVal name As String, ByVal input As SDVariable, ByVal Pooling3DConfig As Pooling3DConfig) As SDVariable
		SDValidation.validateNumerical("maxPooling3d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling3D(sd,input, Pooling3DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Separable 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Separable conv2d depth weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="pointWeights"> Point weights, rank 4 with format [1, 1, inputChannels*depthMultiplier, outputChannels]. May be null (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias, rank 1 with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of separable convolution 2d operation (NUMERIC type) </returns>
	  Public Overridable Function separableConv2d(ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal pointWeights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("separableConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("separableConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("separableConv2d", "pointWeights", pointWeights)
		SDValidation.validateNumerical("separableConv2d", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.SConv2D(sd,layerInput, depthWeights, pointWeights, bias, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Separable 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Separable conv2d depth weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="pointWeights"> Point weights, rank 4 with format [1, 1, inputChannels*depthMultiplier, outputChannels]. May be null (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias, rank 1 with shape [outputChannels]. May be null. (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of separable convolution 2d operation (NUMERIC type) </returns>
	  Public Overridable Function separableConv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal pointWeights As SDVariable, ByVal bias As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("separableConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("separableConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("separableConv2d", "pointWeights", pointWeights)
		SDValidation.validateNumerical("separableConv2d", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.SConv2D(sd,layerInput, depthWeights, pointWeights, bias, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Separable 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Separable conv2d depth weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="pointWeights"> Point weights, rank 4 with format [1, 1, inputChannels*depthMultiplier, outputChannels]. May be null (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of separable convolution 2d operation (NUMERIC type) </returns>
	  Public Overridable Function separableConv2d(ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal pointWeights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("separableConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("separableConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("separableConv2d", "pointWeights", pointWeights)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.SConv2D(sd,layerInput, depthWeights, pointWeights, Nothing, Conv2DConfig)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Separable 2D convolution operation with optional bias <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="layerInput"> the input to max pooling 2d operation - 4d CNN (image) activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="depthWeights"> Separable conv2d depth weights. 4 dimensions with format [kernelHeight, kernelWidth, inputChannels, depthMultiplier] (NUMERIC type) </param>
	  ''' <param name="pointWeights"> Point weights, rank 4 with format [1, 1, inputChannels*depthMultiplier, outputChannels]. May be null (NUMERIC type) </param>
	  ''' <param name="Conv2DConfig"> Configuration Object </param>
	  ''' <returns> output result of separable convolution 2d operation (NUMERIC type) </returns>
	  Public Overridable Function separableConv2d(ByVal name As String, ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal pointWeights As SDVariable, ByVal Conv2DConfig As Conv2DConfig) As SDVariable
		SDValidation.validateNumerical("separableConv2d", "layerInput", layerInput)
		SDValidation.validateNumerical("separableConv2d", "depthWeights", depthWeights)
		SDValidation.validateNumerical("separableConv2d", "pointWeights", pointWeights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.SConv2D(sd,layerInput, depthWeights, pointWeights, Nothing, Conv2DConfig)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer space to batch operation on 4d input.<br>
	  ''' Increases input batch dimension by rearranging data from spatial dimensions into batch dimension <br>
	  ''' </summary>
	  ''' <param name="x"> Input variable. 4d input (NUMERIC type) </param>
	  ''' <param name="blocks"> Block size, in the height/width dimension (Size: Exactly(count=2)) </param>
	  ''' <param name="paddingTop"> Optional 2d int[] array for padding the result: values [[pad top, pad bottom], [pad left, pad right]] (Size: Exactly(count=2)) </param>
	  ''' <param name="paddingBottom"> Optional 2d int[] array for padding the result: values [[pad top, pad bottom], [pad left, pad right]] (Size: Exactly(count=2)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function spaceToBatch(ByVal x As SDVariable, ByVal blocks() As Integer, ByVal paddingTop() As Integer, ParamArray ByVal paddingBottom() As Integer) As SDVariable
		SDValidation.validateNumerical("spaceToBatch", "x", x)
		Preconditions.checkArgument(blocks.Length = 2, "blocks has incorrect size/length. Expected: blocks.length == 2, got %s", blocks.Length)
		Preconditions.checkArgument(paddingTop.Length = 2, "paddingTop has incorrect size/length. Expected: paddingTop.length == 2, got %s", paddingTop.Length)
		Preconditions.checkArgument(paddingBottom.Length = 2, "paddingBottom has incorrect size/length. Expected: paddingBottom.length == 2, got %s", paddingBottom.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.SpaceToBatch(sd,x, blocks, paddingTop, paddingBottom)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer space to batch operation on 4d input.<br>
	  ''' Increases input batch dimension by rearranging data from spatial dimensions into batch dimension <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable. 4d input (NUMERIC type) </param>
	  ''' <param name="blocks"> Block size, in the height/width dimension (Size: Exactly(count=2)) </param>
	  ''' <param name="paddingTop"> Optional 2d int[] array for padding the result: values [[pad top, pad bottom], [pad left, pad right]] (Size: Exactly(count=2)) </param>
	  ''' <param name="paddingBottom"> Optional 2d int[] array for padding the result: values [[pad top, pad bottom], [pad left, pad right]] (Size: Exactly(count=2)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function spaceToBatch(ByVal name As String, ByVal x As SDVariable, ByVal blocks() As Integer, ByVal paddingTop() As Integer, ParamArray ByVal paddingBottom() As Integer) As SDVariable
		SDValidation.validateNumerical("spaceToBatch", "x", x)
		Preconditions.checkArgument(blocks.Length = 2, "blocks has incorrect size/length. Expected: blocks.length == 2, got %s", blocks.Length)
		Preconditions.checkArgument(paddingTop.Length = 2, "paddingTop has incorrect size/length. Expected: paddingTop.length == 2, got %s", paddingTop.Length)
		Preconditions.checkArgument(paddingBottom.Length = 2, "paddingBottom has incorrect size/length. Expected: paddingBottom.length == 2, got %s", paddingBottom.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.SpaceToBatch(sd,x, blocks, paddingTop, paddingBottom)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer space to depth operation on 4d input.<br>
	  ''' Increases input channels (reduced spatial dimensions) by rearranging data into a larger channels dimension<br>
	  ''' Example: if input has shape [mb, 2, 4, 4] and block size is 2, then output size is [mb, 8/(2*2), 2*2, 2*2]<br>
	  ''' = [mb, 2, 4, 4] <br>
	  ''' </summary>
	  ''' <param name="x"> the input to depth to space pooling 2d operation - 4d activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="blockSize">  Block size, in the height/width dimension </param>
	  ''' <param name="dataFormat"> Data format: "NCHW" or "NHWC" </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function spaceToDepth(ByVal x As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat) As SDVariable
		SDValidation.validateNumerical("spaceToDepth", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.SpaceToDepth(sd,x, blockSize, dataFormat)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Convolution 2d layer space to depth operation on 4d input.<br>
	  ''' Increases input channels (reduced spatial dimensions) by rearranging data into a larger channels dimension<br>
	  ''' Example: if input has shape [mb, 2, 4, 4] and block size is 2, then output size is [mb, 8/(2*2), 2*2, 2*2]<br>
	  ''' = [mb, 2, 4, 4] <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> the input to depth to space pooling 2d operation - 4d activations in NCHW format (shape [minibatch, channels, height, width]) or NHWC format (shape [minibatch, height, width, channels]) (NUMERIC type) </param>
	  ''' <param name="blockSize">  Block size, in the height/width dimension </param>
	  ''' <param name="dataFormat"> Data format: "NCHW" or "NHWC" </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function spaceToDepth(ByVal name As String, ByVal x As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat) As SDVariable
		SDValidation.validateNumerical("spaceToDepth", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.SpaceToDepth(sd,x, blockSize, dataFormat)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Upsampling layer for 2D inputs.<br>
	  ''' scale is used for both height and width dimensions. <br>
	  ''' </summary>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="scale"> The scale for both height and width dimensions. </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling2d(ByVal input As SDVariable, ByVal scale As Integer) As SDVariable
		SDValidation.validateNumerical("upsampling2d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling2d(sd,input, scale)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Upsampling layer for 2D inputs.<br>
	  ''' scale is used for both height and width dimensions. <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="scale"> The scale for both height and width dimensions. </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling2d(ByVal name As String, ByVal input As SDVariable, ByVal scale As Integer) As SDVariable
		SDValidation.validateNumerical("upsampling2d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling2d(sd,input, scale)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - Upsampling 2d <br>
	  ''' </summary>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="scaleH"> Scale to upsample in height dimension </param>
	  ''' <param name="scaleW"> Scale to upsample in width dimension </param>
	  ''' <param name="nchw"> If true: input is in NCHW (minibatch, channels, height, width) format. False: NHWC format </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling2d(ByVal input As SDVariable, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal nchw As Boolean) As SDVariable
		SDValidation.validateNumerical("upsampling2d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling2d(sd,input, scaleH, scaleW, nchw)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 2D Convolution layer operation - Upsampling 2d <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="scaleH"> Scale to upsample in height dimension </param>
	  ''' <param name="scaleW"> Scale to upsample in width dimension </param>
	  ''' <param name="nchw"> If true: input is in NCHW (minibatch, channels, height, width) format. False: NHWC format </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling2d(ByVal name As String, ByVal input As SDVariable, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal nchw As Boolean) As SDVariable
		SDValidation.validateNumerical("upsampling2d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling2d(sd,input, scaleH, scaleW, nchw)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' 3D Convolution layer operation - Upsampling 3d <br>
	  ''' </summary>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="ncdhw"> If true: input is in NCDHW (minibatch, channels, depth, height, width) format. False: NDHWC format </param>
	  ''' <param name="scaleD"> Scale to upsample in depth dimension </param>
	  ''' <param name="scaleH"> Scale to upsample in height dimension </param>
	  ''' <param name="scaleW"> Scale to upsample in width dimension </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling3d(ByVal input As SDVariable, ByVal ncdhw As Boolean, ByVal scaleD As Integer, ByVal scaleH As Integer, ByVal scaleW As Integer) As SDVariable
		SDValidation.validateNumerical("upsampling3d", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling3d(sd,input, ncdhw, scaleD, scaleH, scaleW)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' 3D Convolution layer operation - Upsampling 3d <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input in NCHW format (NUMERIC type) </param>
	  ''' <param name="ncdhw"> If true: input is in NCDHW (minibatch, channels, depth, height, width) format. False: NDHWC format </param>
	  ''' <param name="scaleD"> Scale to upsample in depth dimension </param>
	  ''' <param name="scaleH"> Scale to upsample in height dimension </param>
	  ''' <param name="scaleW"> Scale to upsample in width dimension </param>
	  ''' <returns> output Upsampled input (NUMERIC type) </returns>
	  Public Overridable Function upsampling3d(ByVal name As String, ByVal input As SDVariable, ByVal ncdhw As Boolean, ByVal scaleD As Integer, ByVal scaleH As Integer, ByVal scaleW As Integer) As SDVariable
		SDValidation.validateNumerical("upsampling3d", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling3d(sd,input, ncdhw, scaleD, scaleH, scaleW)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace