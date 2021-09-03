import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports LSTMConfiguration = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration
Imports LSTMLayerConfig = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports GRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights
Imports LSTMLayerWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports LSTMWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights
Imports SRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights

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

	Public Class SDRNN
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' The GRU operation. Gated Recurrent Unit - Cho et al. 2014.<br>
	  ''' </summary>
	  ''' <param name="x"> input [time, bS, nIn] (NUMERIC type) </param>
	  ''' <param name="hLast"> initial cell output (at time step = 0) [bS, nOut] (NUMERIC type) </param>
	  ''' <param name="Wx"> input-to-hidden  weights, [nIn, 3*nOut] (NUMERIC type) </param>
	  ''' <param name="Wh"> hidden-to-hidden weights, [nOut, 3*nOut] (NUMERIC type) </param>
	  ''' <param name="biases"> biases, [3*nOut] (NUMERIC type) </param>
	  ''' <returns> h cell outputs [time, bS, nOut], that is per each time step (NUMERIC type) </returns>
	  Public Overridable Function gru(ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal Wx As SDVariable, ByVal Wh As SDVariable, ByVal biases As SDVariable) As SDVariable
		SDValidation.validateNumerical("gru", "x", x)
		SDValidation.validateNumerical("gru", "hLast", hLast)
		SDValidation.validateNumerical("gru", "Wx", Wx)
		SDValidation.validateNumerical("gru", "Wh", Wh)
		SDValidation.validateNumerical("gru", "biases", biases)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRU(sd,x, hLast, Wx, Wh, biases)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The GRU operation. Gated Recurrent Unit - Cho et al. 2014.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> input [time, bS, nIn] (NUMERIC type) </param>
	  ''' <param name="hLast"> initial cell output (at time step = 0) [bS, nOut] (NUMERIC type) </param>
	  ''' <param name="Wx"> input-to-hidden  weights, [nIn, 3*nOut] (NUMERIC type) </param>
	  ''' <param name="Wh"> hidden-to-hidden weights, [nOut, 3*nOut] (NUMERIC type) </param>
	  ''' <param name="biases"> biases, [3*nOut] (NUMERIC type) </param>
	  ''' <returns> h cell outputs [time, bS, nOut], that is per each time step (NUMERIC type) </returns>
	  Public Overridable Function gru(ByVal name As String, ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal Wx As SDVariable, ByVal Wh As SDVariable, ByVal biases As SDVariable) As SDVariable
		SDValidation.validateNumerical("gru", "x", x)
		SDValidation.validateNumerical("gru", "hLast", hLast)
		SDValidation.validateNumerical("gru", "Wx", Wx)
		SDValidation.validateNumerical("gru", "Wh", Wh)
		SDValidation.validateNumerical("gru", "biases", biases)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRU(sd,x, hLast, Wx, Wh, biases)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The GRU cell.  Does a single time step operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="hLast"> Output of the previous cell/time step, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="GRUWeights"> Configuration Object </param>
	  Public Overridable Function gruCell(ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal GRUWeights As GRUWeights) As SDVariable()
		SDValidation.validateNumerical("gruCell", "x", x)
		SDValidation.validateNumerical("gruCell", "hLast", hLast)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell(sd,x, hLast, GRUWeights)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' The GRU cell.  Does a single time step operation<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="hLast"> Output of the previous cell/time step, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="GRUWeights"> Configuration Object </param>
	  Public Overridable Function gruCell(ByVal names() As String, ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal GRUWeights As GRUWeights) As SDVariable()
		SDValidation.validateNumerical("gruCell", "x", x)
		SDValidation.validateNumerical("gruCell", "hLast", hLast)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell(sd,x, hLast, GRUWeights)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' The LSTM cell.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> revious cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  Public Overridable Function lstmCell(ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable()
		SDValidation.validateNumerical("lstmCell", "x", x)
		SDValidation.validateNumerical("lstmCell", "cLast", cLast)
		SDValidation.validateNumerical("lstmCell", "yLast", yLast)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell(sd,x, cLast, yLast, LSTMWeights, LSTMConfiguration)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' The LSTM cell.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> revious cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  Public Overridable Function lstmCell(ByVal names() As String, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable()
		SDValidation.validateNumerical("lstmCell", "x", x)
		SDValidation.validateNumerical("lstmCell", "cLast", cLast)
		SDValidation.validateNumerical("lstmCell", "yLast", yLast)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell(sd,x, cLast, yLast, LSTMWeights, LSTMConfiguration)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Long Short-Term Memory layer - Hochreiter 1997.<br>
	  ''' SUPPORTS following data formats:<br>
	  ''' for unidirectional:<br>
	  ''' TNS: shapes [timeLength, numExamples, inOutSize]<br>
	  ''' NST: shapes [numExamples, inOutSize, timeLength]<br>
	  ''' NTS: shapes [numExamples, timeLength, inOutSize]<br>
	  ''' for bidirectional:<br>
	  ''' T2NS: shapes [timeLength, 2, numExamples, inOutSize] (for ONNX)<br>
	  ''' SUPPORTS following direction modes:<br>
	  ''' FWD: forward<br>
	  ''' BWD: backward<br>
	  ''' BIDIR_SUM: bidirectional sum<br>
	  ''' BIDIR_CONCAT: bidirectional concat<br>
	  ''' BIDIR_EXTRA_DIM: bidirectional extra output dim (in conjunction with format dataFormat - T2NS)<br>
	  ''' You may use different gate configurations:<br>
	  ''' specify gate/cell/out aplha/beta and numbers of activations for gate/cell/out described in activations enum<br>
	  ''' ("RELU","SIGMOID","AFFINE","LEAKY_RELU","THRESHHOLD_RELU","SCALED_TAHN","HARD_SIGMOID","ELU","SOFTSIGN","SOFTPLUS")<br>
	  ''' Also this layer supports MKLDNN (DNNL) and cuDNN acceleration<br>
	  ''' </summary>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous/initial cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> Previous/initial cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="maxTSLength"> maxTSLength with shape [batchSize] (NUMERIC type) </param>
	  ''' <param name="LSTMLayerWeights"> Configuration Object </param>
	  ''' <param name="LSTMLayerConfig"> Configuration Object </param>
	  Public Overridable Function lstmLayer(ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal maxTSLength As SDVariable, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As SDVariable()
		SDValidation.validateNumerical("lstmLayer", "x", x)
		SDValidation.validateNumerical("lstmLayer", "cLast", cLast)
		SDValidation.validateNumerical("lstmLayer", "yLast", yLast)
		SDValidation.validateNumerical("lstmLayer", "maxTSLength", maxTSLength)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(sd,x, cLast, yLast, maxTSLength, LSTMLayerWeights, LSTMLayerConfig)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Long Short-Term Memory layer - Hochreiter 1997.<br>
	  ''' SUPPORTS following data formats:<br>
	  ''' for unidirectional:<br>
	  ''' TNS: shapes [timeLength, numExamples, inOutSize]<br>
	  ''' NST: shapes [numExamples, inOutSize, timeLength]<br>
	  ''' NTS: shapes [numExamples, timeLength, inOutSize]<br>
	  ''' for bidirectional:<br>
	  ''' T2NS: shapes [timeLength, 2, numExamples, inOutSize] (for ONNX)<br>
	  ''' SUPPORTS following direction modes:<br>
	  ''' FWD: forward<br>
	  ''' BWD: backward<br>
	  ''' BIDIR_SUM: bidirectional sum<br>
	  ''' BIDIR_CONCAT: bidirectional concat<br>
	  ''' BIDIR_EXTRA_DIM: bidirectional extra output dim (in conjunction with format dataFormat - T2NS)<br>
	  ''' You may use different gate configurations:<br>
	  ''' specify gate/cell/out aplha/beta and numbers of activations for gate/cell/out described in activations enum<br>
	  ''' ("RELU","SIGMOID","AFFINE","LEAKY_RELU","THRESHHOLD_RELU","SCALED_TAHN","HARD_SIGMOID","ELU","SOFTSIGN","SOFTPLUS")<br>
	  ''' Also this layer supports MKLDNN (DNNL) and cuDNN acceleration<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous/initial cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> Previous/initial cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="maxTSLength"> maxTSLength with shape [batchSize] (NUMERIC type) </param>
	  ''' <param name="LSTMLayerWeights"> Configuration Object </param>
	  ''' <param name="LSTMLayerConfig"> Configuration Object </param>
	  Public Overridable Function lstmLayer(ByVal names() As String, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal maxTSLength As SDVariable, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As SDVariable()
		SDValidation.validateNumerical("lstmLayer", "x", x)
		SDValidation.validateNumerical("lstmLayer", "cLast", cLast)
		SDValidation.validateNumerical("lstmLayer", "yLast", yLast)
		SDValidation.validateNumerical("lstmLayer", "maxTSLength", maxTSLength)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(sd,x, cLast, yLast, maxTSLength, LSTMLayerWeights, LSTMLayerConfig)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Long Short-Term Memory layer - Hochreiter 1997.<br>
	  ''' SUPPORTS following data formats:<br>
	  ''' for unidirectional:<br>
	  ''' TNS: shapes [timeLength, numExamples, inOutSize]<br>
	  ''' NST: shapes [numExamples, inOutSize, timeLength]<br>
	  ''' NTS: shapes [numExamples, timeLength, inOutSize]<br>
	  ''' for bidirectional:<br>
	  ''' T2NS: shapes [timeLength, 2, numExamples, inOutSize] (for ONNX)<br>
	  ''' SUPPORTS following direction modes:<br>
	  ''' FWD: forward<br>
	  ''' BWD: backward<br>
	  ''' BIDIR_SUM: bidirectional sum<br>
	  ''' BIDIR_CONCAT: bidirectional concat<br>
	  ''' BIDIR_EXTRA_DIM: bidirectional extra output dim (in conjunction with format dataFormat - T2NS)<br>
	  ''' You may use different gate configurations:<br>
	  ''' specify gate/cell/out aplha/beta and numbers of activations for gate/cell/out described in activations enum<br>
	  ''' ("RELU","SIGMOID","AFFINE","LEAKY_RELU","THRESHHOLD_RELU","SCALED_TAHN","HARD_SIGMOID","ELU","SOFTSIGN","SOFTPLUS")<br>
	  ''' Also this layer supports MKLDNN (DNNL) and cuDNN acceleration<br>
	  ''' </summary>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="LSTMLayerWeights"> Configuration Object </param>
	  ''' <param name="LSTMLayerConfig"> Configuration Object </param>
	  Public Overridable Function lstmLayer(ByVal x As SDVariable, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As SDVariable()
		SDValidation.validateNumerical("lstmLayer", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(sd,x, Nothing, Nothing, Nothing, LSTMLayerWeights, LSTMLayerConfig)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Long Short-Term Memory layer - Hochreiter 1997.<br>
	  ''' SUPPORTS following data formats:<br>
	  ''' for unidirectional:<br>
	  ''' TNS: shapes [timeLength, numExamples, inOutSize]<br>
	  ''' NST: shapes [numExamples, inOutSize, timeLength]<br>
	  ''' NTS: shapes [numExamples, timeLength, inOutSize]<br>
	  ''' for bidirectional:<br>
	  ''' T2NS: shapes [timeLength, 2, numExamples, inOutSize] (for ONNX)<br>
	  ''' SUPPORTS following direction modes:<br>
	  ''' FWD: forward<br>
	  ''' BWD: backward<br>
	  ''' BIDIR_SUM: bidirectional sum<br>
	  ''' BIDIR_CONCAT: bidirectional concat<br>
	  ''' BIDIR_EXTRA_DIM: bidirectional extra output dim (in conjunction with format dataFormat - T2NS)<br>
	  ''' You may use different gate configurations:<br>
	  ''' specify gate/cell/out aplha/beta and numbers of activations for gate/cell/out described in activations enum<br>
	  ''' ("RELU","SIGMOID","AFFINE","LEAKY_RELU","THRESHHOLD_RELU","SCALED_TAHN","HARD_SIGMOID","ELU","SOFTSIGN","SOFTPLUS")<br>
	  ''' Also this layer supports MKLDNN (DNNL) and cuDNN acceleration<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="LSTMLayerWeights"> Configuration Object </param>
	  ''' <param name="LSTMLayerConfig"> Configuration Object </param>
	  Public Overridable Function lstmLayer(ByVal names() As String, ByVal x As SDVariable, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As SDVariable()
		SDValidation.validateNumerical("lstmLayer", "x", x)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(sd,x, Nothing, Nothing, Nothing, LSTMLayerWeights, LSTMLayerConfig)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' The LSTM block<br>
	  ''' </summary>
	  ''' <param name="maxTSLength">  (NUMERIC type) </param>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous/initial cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> Previous/initial cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  ''' <returns> output The layer's outputs. (NUMERIC type) </returns>
	  Public Overridable Function lstmblock(ByVal maxTSLength As SDVariable, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable
		SDValidation.validateNumerical("lstmblock", "maxTSLength", maxTSLength)
		SDValidation.validateNumerical("lstmblock", "x", x)
		SDValidation.validateNumerical("lstmblock", "cLast", cLast)
		SDValidation.validateNumerical("lstmblock", "yLast", yLast)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(sd,maxTSLength, x, cLast, yLast, LSTMWeights, LSTMConfiguration)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The LSTM block<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="maxTSLength">  (NUMERIC type) </param>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous/initial cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> Previous/initial cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  ''' <returns> output The layer's outputs. (NUMERIC type) </returns>
	  Public Overridable Function lstmblock(ByVal name As String, ByVal maxTSLength As SDVariable, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable
		SDValidation.validateNumerical("lstmblock", "maxTSLength", maxTSLength)
		SDValidation.validateNumerical("lstmblock", "x", x)
		SDValidation.validateNumerical("lstmblock", "cLast", cLast)
		SDValidation.validateNumerical("lstmblock", "yLast", yLast)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(sd,maxTSLength, x, cLast, yLast, LSTMWeights, LSTMConfiguration)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The LSTM block<br>
	  ''' </summary>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  ''' <returns> output The layer's outputs. (NUMERIC type) </returns>
	  Public Overridable Function lstmblock(ByVal x As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable
		SDValidation.validateNumerical("lstmblock", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(sd,Nothing, x, Nothing, Nothing, LSTMWeights, LSTMConfiguration)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The LSTM block<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  ''' <returns> output The layer's outputs. (NUMERIC type) </returns>
	  Public Overridable Function lstmblock(ByVal name As String, ByVal x As SDVariable, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As SDVariable
		SDValidation.validateNumerical("lstmblock", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(sd,Nothing, x, Nothing, Nothing, LSTMWeights, LSTMConfiguration)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="mask"> An optional dropout mask, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal mask As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sru", "x", x)
		SDValidation.validateNumerical("sru", "initialC", initialC)
		SDValidation.validateNumerical("sru", "mask", mask)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(sd,x, initialC, mask, SRUWeights)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="mask"> An optional dropout mask, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal name As String, ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal mask As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sru", "x", x)
		SDValidation.validateNumerical("sru", "initialC", initialC)
		SDValidation.validateNumerical("sru", "mask", mask)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(sd,x, initialC, mask, SRUWeights)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sru", "x", x)
		SDValidation.validateNumerical("sru", "initialC", initialC)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(sd,x, initialC, Nothing, SRUWeights)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal name As String, ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sru", "x", x)
		SDValidation.validateNumerical("sru", "initialC", initialC)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(sd,x, initialC, Nothing, SRUWeights)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs. (NUMERIC type) </returns>
	  Public Overridable Function sruCell(ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sruCell", "x", x)
		SDValidation.validateNumerical("sruCell", "cLast", cLast)
		Return (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRUCell(sd,x, cLast, SRUWeights)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs. (NUMERIC type) </returns>
	  Public Overridable Function sruCell(ByVal name As String, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal SRUWeights As SRUWeights) As SDVariable
		SDValidation.validateNumerical("sruCell", "x", x)
		SDValidation.validateNumerical("sruCell", "cLast", cLast)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRUCell(sd,x, cLast, SRUWeights)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace