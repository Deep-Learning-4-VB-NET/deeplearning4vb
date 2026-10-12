Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LSTMConfiguration = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration
Imports LSTMCellOutputs = org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.LSTMCellOutputs
Imports GRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights
Imports LSTMWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.autodiff.opvalidation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class RnnOpValidation extends BaseOpValidation
	Public Class RnnOpValidation
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRnnBlockCell(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRnnBlockCell(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim mb As Integer = 2
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, mb, nIn))
			Dim cLast As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, mb, nOut))
			Dim yLast As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, mb, nOut))
			Dim W As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, (nIn+nOut), 4*nOut))
			Dim Wci As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, nOut))
			Dim Wcf As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, nOut))
			Dim Wco As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, nOut))
			Dim b As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, 4*nOut))

			Dim fb As Double = 1.0
			Dim conf As LSTMConfiguration = LSTMConfiguration.builder().peepHole(True).forgetBias(fb).clippingCellValue(0.0).build()

			Dim weights As LSTMWeights = LSTMWeights.builder().weights(W).bias(b).inputPeepholeWeights(Wci).forgetPeepholeWeights(Wcf).outputPeepholeWeights(Wco).build()

			Dim v As New LSTMCellOutputs(sd.rnn().lstmCell(x, cLast, yLast, weights, conf)) 'Output order: i, c, f, o, z, h, y
			Dim toExec As IList(Of String) = New List(Of String)()
			For Each sdv As SDVariable In v.getAllOutputs()
				toExec.Add(sdv.name())
			Next sdv

			'Test forward pass:
			Dim m As IDictionary(Of String, INDArray) = sd.output(Nothing, toExec)

			'Weights and bias order: [i, f, z, o]

			'Block input (z) - post tanh:
			Dim wz_x As INDArray = W.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(nOut, 2*nOut)) 'Input weights
			Dim wz_r As INDArray = W.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(nOut, 2*nOut)) 'Recurrent weights
			Dim bz As INDArray = b.Arr.get(NDArrayIndex.interval(nOut, 2*nOut))

			Dim zExp As INDArray = x.Arr.mmul(wz_x).addiRowVector(bz) '[mb,nIn]*[nIn, nOut] + [nOut]
			zExp.addi(yLast.Arr.mmul(wz_r)) '[mb,nOut]*[nOut,nOut]
			Transforms.tanh(zExp, False)

			Dim zAct As INDArray = m(toExec(4))
			assertEquals(zExp, zAct)

			'Input modulation gate (post sigmoid) - i: (note: peephole input - last time step)
			Dim wi_x As INDArray = W.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(0, nOut)) 'Input weights
			Dim wi_r As INDArray = W.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(0, nOut)) 'Recurrent weights
			Dim bi As INDArray = b.Arr.get(NDArrayIndex.interval(0, nOut))

			Dim iExp As INDArray = x.Arr.mmul(wi_x).addiRowVector(bi) '[mb,nIn]*[nIn, nOut] + [nOut]
			iExp.addi(yLast.Arr.mmul(wi_r)) '[mb,nOut]*[nOut,nOut]
			iExp.addi(cLast.Arr.mulRowVector(Wci.Arr)) 'Peephole
			Transforms.sigmoid(iExp, False)
			assertEquals(iExp, m(toExec(0)))

			'Forget gate (post sigmoid): (note: peephole input - last time step)
			Dim wf_x As INDArray = W.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(2*nOut, 3*nOut)) 'Input weights
			Dim wf_r As INDArray = W.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(2*nOut, 3*nOut)) 'Recurrent weights
			Dim bf As INDArray = b.Arr.get(NDArrayIndex.interval(2*nOut, 3*nOut))

			Dim fExp As INDArray = x.Arr.mmul(wf_x).addiRowVector(bf) '[mb,nIn]*[nIn, nOut] + [nOut]
			fExp.addi(yLast.Arr.mmul(wf_r)) '[mb,nOut]*[nOut,nOut]
			fExp.addi(cLast.Arr.mulRowVector(Wcf.Arr)) 'Peephole
			fExp.addi(fb)
			Transforms.sigmoid(fExp, False)
			assertEquals(fExp, m(toExec(2)))

			'Cell state (pre tanh): tanh(z) .* sigmoid(i) + sigmoid(f) .* cLast
			Dim cExp As INDArray = zExp.mul(iExp).add(fExp.mul(cLast.Arr))
			Dim cAct As INDArray = m(toExec(1))
			assertEquals(cExp, cAct)

			'Output gate (post sigmoid): (note: peephole input: current time step)
			Dim wo_x As INDArray = W.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(3*nOut, 4*nOut)) 'Input weights
			Dim wo_r As INDArray = W.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(3*nOut, 4*nOut)) 'Recurrent weights
			Dim bo As INDArray = b.Arr.get(NDArrayIndex.interval(3*nOut, 4*nOut))

			Dim oExp As INDArray = x.Arr.mmul(wo_x).addiRowVector(bo) '[mb,nIn]*[nIn, nOut] + [nOut]
			oExp.addi(yLast.Arr.mmul(wo_r)) '[mb,nOut]*[nOut,nOut]
			oExp.addi(cExp.mulRowVector(Wco.Arr)) 'Peephole
			Transforms.sigmoid(oExp, False)
			assertEquals(oExp, m(toExec(3)))

			'Cell state, post tanh
			Dim hExp As INDArray = Transforms.tanh(cExp, True)
			assertEquals(hExp, m(toExec(5)))

			'Final output
			Dim yExp As INDArray = hExp.mul(oExp)
			assertEquals(yExp, m(toExec(6)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRnnBlockCellManualTFCompare(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRnnBlockCellManualTFCompare(ByVal backend As Nd4jBackend)
			'Test case: "rnn/lstmblockcell/static_batch1_n3-2_tsLength1_noPH_noClip_fBias1_noIS"

			Dim sd As SameDiff = SameDiff.create()
			Dim zero2d As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {0, 0}
			})
			Dim zero1d As INDArray = Nd4j.createFromArray(New Single(){0, 0})
			Dim x As SDVariable = sd.constant(Nd4j.createFromArray(New Single()(){
				New Single() {0.7787856f, 0.80119777f, 0.72437465f}
			}))
			Dim cLast As SDVariable = sd.constant(zero2d)
			Dim yLast As SDVariable = sd.constant(zero2d)
			'Weights shape: [(nIn+nOut), 4*nOut]
			Dim W As SDVariable = sd.constant(Nd4j.createFromArray(-0.61977,-0.5708851,-0.38089648,-0.07994056,-0.31706482,0.21500933,-0.35454142,-0.3239095,-0.3177906, 0.39918554,-0.3115911,0.540841,0.38552666,0.34270835,-0.63456273,-0.13917702,-0.2985368,0.343238, -0.3178353,0.017154932,-0.060259163,0.28841054,-0.6257687,0.65097713,0.24375653,-0.22315514,0.2033832, 0.24894875,-0.2062299,-0.2242794,-0.3809483,-0.023048997,-0.036284804,-0.46398938,-0.33979666,0.67012596, -0.42168984,0.34208286,-0.0456419,0.39803517).castTo(DataType.FLOAT).reshape(5,8))
			Dim Wci As SDVariable = sd.constant(zero1d)
			Dim Wcf As SDVariable = sd.constant(zero1d)
			Dim Wco As SDVariable = sd.constant(zero1d)
			Dim b As SDVariable = sd.constant(Nd4j.zeros(DataType.FLOAT, 8))

			Dim fb As Double = 1.0
			Dim conf As LSTMConfiguration = LSTMConfiguration.builder().peepHole(False).forgetBias(fb).clippingCellValue(0.0).build()

			Dim weights As LSTMWeights = LSTMWeights.builder().weights(W).bias(b).inputPeepholeWeights(Wci).forgetPeepholeWeights(Wcf).outputPeepholeWeights(Wco).build()

			Dim v As New LSTMCellOutputs(sd.rnn().lstmCell(x, cLast, yLast, weights, conf)) 'Output order: i, c, f, o, z, h, y
			Dim toExec As IList(Of String) = New List(Of String)()
			For Each sdv As SDVariable In v.getAllOutputs()
				toExec.Add(sdv.name())
			Next sdv

			'Test forward pass:
			Dim m As IDictionary(Of String, INDArray) = sd.output(Nothing, toExec)

			Dim out0 As INDArray = Nd4j.create(New Single(){0.27817473f, 0.53092605f}, New Integer(){1, 2}) 'Input mod gate
			Dim out1 As INDArray = Nd4j.create(New Single(){-0.18100877f, 0.19417824f}, New Integer(){1, 2}) 'CS (pre tanh)
			Dim out2 As INDArray = Nd4j.create(New Single(){0.73464274f, 0.83901811f}, New Integer(){1, 2}) 'Forget gate
			Dim out3 As INDArray = Nd4j.create(New Single(){0.22481689f, 0.52692068f}, New Integer(){1, 2}) 'Output gate

			Dim out4 As INDArray = Nd4j.create(New Single(){-0.65070170f, 0.36573499f}, New Integer(){1, 2}) 'block input
			Dim out5 As INDArray = Nd4j.create(New Single(){-0.17905743f, 0.19177397f}, New Integer(){1, 2}) 'Cell state
			Dim out6 As INDArray = Nd4j.create(New Single(){-0.04025514f, 0.10104967f}, New Integer(){1, 2}) 'Output

	'        for(int i=0; i<toExec.size(); i++ ){
	'            System.out.println(i + "\t" + m.get(toExec.get(i)));
	'        }

			assertEquals(out0, m(toExec(0))) 'Input modulation gate
			assertEquals(out1, m(toExec(1))) 'Cell state (pre tanh)
			assertEquals(out2, m(toExec(2))) 'Forget gate
			assertEquals(out3, m(toExec(3))) 'Output gate
			assertEquals(out4, m(toExec(4))) 'block input
			assertEquals(out5, m(toExec(5))) 'Cell state
			assertEquals(out6, m(toExec(6))) 'Output
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGRUCell()
		Public Overridable Sub testGRUCell()
			Nd4j.Random.setSeed(12345)
			Dim mb As Integer = 2
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, mb, nIn))
			Dim hLast As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, mb, nOut))
			Dim Wru As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, (nIn+nOut), 2*nOut))
			Dim Wc As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, (nIn+nOut), nOut))
			Dim bru As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, 2*nOut))
			Dim bc As SDVariable = sd.constant(Nd4j.rand(DataType.FLOAT, nOut))

			Dim fb As Double = 1.0
			Dim weights As GRUWeights = GRUWeights.builder().ruWeight(Wru).cWeight(Wc).ruBias(bru).cBias(bc).build()

			Dim v() As SDVariable = sd.rnn().gruCell(x, hLast, weights)
			Dim toExec As IList(Of String) = New List(Of String)()
			For Each sdv As SDVariable In v
				toExec.Add(sdv.name())
			Next sdv

			'Test forward pass:
			Dim m As IDictionary(Of String, INDArray) = sd.output(Nothing, toExec)

			'Weights and bias order: [r, u], [c]

			'Reset gate:
			Dim wr_x As INDArray = Wru.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(0, nOut)) 'Input weights
			Dim wr_r As INDArray = Wru.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(0, nOut)) 'Recurrent weights
			Dim br As INDArray = bru.Arr.get(NDArrayIndex.interval(0, nOut))

			Dim rExp As INDArray = x.Arr.mmul(wr_x).addiRowVector(br) '[mb,nIn]*[nIn, nOut] + [nOut]
			rExp.addi(hLast.Arr.mmul(wr_r)) '[mb,nOut]*[nOut,nOut]
			Transforms.sigmoid(rExp,False)

			Dim rAct As INDArray = m(toExec(0))
			assertEquals(rExp, rAct)

			'Update gate:
			Dim wu_x As INDArray = Wru.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.interval(nOut, 2*nOut)) 'Input weights
			Dim wu_r As INDArray = Wru.Arr.get(NDArrayIndex.interval(nIn,nIn+nOut), NDArrayIndex.interval(nOut, 2*nOut)) 'Recurrent weights
			Dim bu As INDArray = bru.Arr.get(NDArrayIndex.interval(nOut, 2*nOut))

			Dim uExp As INDArray = x.Arr.mmul(wu_x).addiRowVector(bu) '[mb,nIn]*[nIn, nOut] + [nOut]
			uExp.addi(hLast.Arr.mmul(wu_r)) '[mb,nOut]*[nOut,nOut]
			Transforms.sigmoid(uExp,False)

			Dim uAct As INDArray = m(toExec(1))
			assertEquals(uExp, uAct)

			'c = tanh(x * Wcx + Wcr * (hLast .* r))
			Dim Wcx As INDArray = Wc.Arr.get(NDArrayIndex.interval(0,nIn), NDArrayIndex.all())
			Dim Wcr As INDArray = Wc.Arr.get(NDArrayIndex.interval(nIn, nIn+nOut), NDArrayIndex.all())
			Dim cExp As INDArray = x.Arr.mmul(Wcx)
			cExp.addi(hLast.Arr.mul(rExp).mmul(Wcr))
			cExp.addiRowVector(bc.Arr)
			Transforms.tanh(cExp, False)

			assertEquals(cExp, m(toExec(2)))

			'h = u * hLast + (1-u) * c
			Dim hExp As INDArray = uExp.mul(hLast.Arr).add(uExp.rsub(1.0).mul(cExp))
			assertEquals(hExp, m(toExec(3)))
		End Sub
	End Class
End Namespace