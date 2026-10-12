import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LSTMConfiguration = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration
Imports LSTMLayerConfig = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports GRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights
Imports LSTMLayerWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports LSTMWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights
Imports SRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.factory.ops

	Public Class NDRNN
	  Public Sub New()
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
	  Public Overridable Function gru(ByVal x As INDArray, ByVal hLast As INDArray, ByVal Wx As INDArray, ByVal Wh As INDArray, ByVal biases As INDArray) As INDArray
		NDValidation.validateNumerical("gru", "x", x)
		NDValidation.validateNumerical("gru", "hLast", hLast)
		NDValidation.validateNumerical("gru", "Wx", Wx)
		NDValidation.validateNumerical("gru", "Wh", Wh)
		NDValidation.validateNumerical("gru", "biases", biases)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRU(x, hLast, Wx, Wh, biases))(0)
	  End Function

	  ''' <summary>
	  ''' The GRU cell.  Does a single time step operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="hLast"> Output of the previous cell/time step, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="GRUWeights"> Configuration Object </param>
	  Public Overridable Function gruCell(ByVal x As INDArray, ByVal hLast As INDArray, ByVal GRUWeights As GRUWeights) As INDArray()
		NDValidation.validateNumerical("gruCell", "x", x)
		NDValidation.validateNumerical("gruCell", "hLast", hLast)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell(x, hLast, GRUWeights))
	  End Function

	  ''' <summary>
	  ''' The LSTM cell.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="yLast"> revious cell output, with shape [batchSize, numUnits] (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  Public Overridable Function lstmCell(ByVal x As INDArray, ByVal cLast As INDArray, ByVal yLast As INDArray, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As INDArray()
		NDValidation.validateNumerical("lstmCell", "x", x)
		NDValidation.validateNumerical("lstmCell", "cLast", cLast)
		NDValidation.validateNumerical("lstmCell", "yLast", yLast)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell(x, cLast, yLast, LSTMWeights, LSTMConfiguration))
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
	  Public Overridable Function lstmLayer(ByVal x As INDArray, ByVal cLast As INDArray, ByVal yLast As INDArray, ByVal maxTSLength As INDArray, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As INDArray()
		NDValidation.validateNumerical("lstmLayer", "x", x)
		NDValidation.validateNumerical("lstmLayer", "cLast", cLast)
		NDValidation.validateNumerical("lstmLayer", "yLast", yLast)
		NDValidation.validateNumerical("lstmLayer", "maxTSLength", maxTSLength)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(x, cLast, yLast, maxTSLength, LSTMLayerWeights, LSTMLayerConfig))
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
	  Public Overridable Function lstmLayer(ByVal x As INDArray, ByVal LSTMLayerWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig) As INDArray()
		NDValidation.validateNumerical("lstmLayer", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer(x, Nothing, Nothing, Nothing, LSTMLayerWeights, LSTMLayerConfig))
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
	  Public Overridable Function lstmblock(ByVal maxTSLength As INDArray, ByVal x As INDArray, ByVal cLast As INDArray, ByVal yLast As INDArray, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As INDArray
		NDValidation.validateNumerical("lstmblock", "maxTSLength", maxTSLength)
		NDValidation.validateNumerical("lstmblock", "x", x)
		NDValidation.validateNumerical("lstmblock", "cLast", cLast)
		NDValidation.validateNumerical("lstmblock", "yLast", yLast)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(maxTSLength, x, cLast, yLast, LSTMWeights, LSTMConfiguration))(0)
	  End Function

	  ''' <summary>
	  ''' The LSTM block<br>
	  ''' </summary>
	  ''' <param name="x">  Input, with shape dependent on the data format (in config). (NUMERIC type) </param>
	  ''' <param name="LSTMWeights"> Configuration Object </param>
	  ''' <param name="LSTMConfiguration"> Configuration Object </param>
	  ''' <returns> output The layer's outputs. (NUMERIC type) </returns>
	  Public Overridable Function lstmblock(ByVal x As INDArray, ByVal LSTMWeights As LSTMWeights, ByVal LSTMConfiguration As LSTMConfiguration) As INDArray
		NDValidation.validateNumerical("lstmblock", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlock(Nothing, x, Nothing, Nothing, LSTMWeights, LSTMConfiguration))(0)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="mask"> An optional dropout mask, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal x As INDArray, ByVal initialC As INDArray, ByVal mask As INDArray, ByVal SRUWeights As SRUWeights) As INDArray
		NDValidation.validateNumerical("sru", "x", x)
		NDValidation.validateNumerical("sru", "initialC", initialC)
		NDValidation.validateNumerical("sru", "mask", mask)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(x, initialC, mask, SRUWeights))(0)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="initialC"> Initial cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs.. (NUMERIC type) </returns>
	  Public Overridable Function sru(ByVal x As INDArray, ByVal initialC As INDArray, ByVal SRUWeights As SRUWeights) As INDArray
		NDValidation.validateNumerical("sru", "x", x)
		NDValidation.validateNumerical("sru", "initialC", initialC)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU(x, initialC, Nothing, SRUWeights))(0)
	  End Function

	  ''' <summary>
	  ''' The SRU layer.  Does a single time step operation.<br>
	  ''' </summary>
	  ''' <param name="x"> Input, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="cLast"> Previous cell state, with shape [batchSize, inSize] (NUMERIC type) </param>
	  ''' <param name="SRUWeights"> Configuration Object </param>
	  ''' <returns> output The cell's outputs. (NUMERIC type) </returns>
	  Public Overridable Function sruCell(ByVal x As INDArray, ByVal cLast As INDArray, ByVal SRUWeights As SRUWeights) As INDArray
		NDValidation.validateNumerical("sruCell", "x", x)
		NDValidation.validateNumerical("sruCell", "cLast", cLast)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.recurrent.SRUCell(x, cLast, SRUWeights))(0)
	  End Function
	End Class

End Namespace