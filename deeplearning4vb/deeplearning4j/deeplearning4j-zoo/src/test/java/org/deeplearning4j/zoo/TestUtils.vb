Imports System
Imports System.IO
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.zoo


	Public Class TestUtils

		Public Shared Function testModelSerialization(ByVal net As MultiLayerNetwork) As MultiLayerNetwork

			Try
				Dim baos As New MemoryStream()
				ModelSerializer.writeModel(net, baos, True)
				Dim bytes() As SByte = baos.toByteArray()

				Dim bais As New MemoryStream(bytes)
				Dim restored As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(bais, True)

				assertEquals(net.LayerWiseConfigurations, restored.LayerWiseConfigurations)
				assertEquals(net.params(), restored.params())

				Return restored
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function testModelSerialization(ByVal net As ComputationGraph) As ComputationGraph

			Try
				Dim baos As New MemoryStream()
				ModelSerializer.writeModel(net, baos, True)
				Dim bytes() As SByte = baos.toByteArray()

				Dim bais As New MemoryStream(bytes)
				Dim restored As ComputationGraph = ModelSerializer.restoreComputationGraph(bais, True)

				assertEquals(net.Configuration, restored.Configuration)
				assertEquals(net.params(), restored.params())

				Return restored
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function randomOneHot(ByVal examples As Integer, ByVal nOut As Integer) As INDArray
			Return randomOneHot(examples, nOut, New Random(12345))
		End Function

		Public Shared Function randomOneHot(ByVal examples As Integer, ByVal nOut As Integer, ByVal rngSeed As Long) As INDArray
			Return randomOneHot(examples, nOut, New Random(rngSeed))
		End Function

		Public Shared Function randomOneHot(ByVal examples As Integer, ByVal nOut As Integer, ByVal rng As Random) As INDArray
			Dim arr As INDArray = Nd4j.create(examples, nOut)
			For i As Integer = 0 To examples - 1
				arr.putScalar(i, rng.Next(nOut), 1.0)
			Next i
			Return arr
		End Function

		Public Shared Function randomOneHotTimeSeries(ByVal minibatch As Integer, ByVal outSize As Integer, ByVal tsLength As Integer) As INDArray
			Return randomOneHotTimeSeries(minibatch, outSize, tsLength, New Random())
		End Function

		Public Shared Function randomOneHotTimeSeries(ByVal minibatch As Integer, ByVal outSize As Integer, ByVal tsLength As Integer, ByVal rngSeed As Long) As INDArray
			Return randomOneHotTimeSeries(minibatch, outSize, tsLength, New Random(rngSeed))
		End Function

		Public Shared Function randomOneHotTimeSeries(ByVal minibatch As Integer, ByVal outSize As Integer, ByVal tsLength As Integer, ByVal rng As Random) As INDArray
			Dim [out] As INDArray = Nd4j.create(New Integer(){minibatch, outSize, tsLength}, "f"c)
			For i As Integer = 0 To minibatch - 1
				For j As Integer = 0 To tsLength - 1
					[out].putScalar(i, rng.Next(outSize), j, 1.0)
				Next j
			Next i
			Return [out]
		End Function

		Public Shared Function randomBernoulli(ParamArray ByVal shape() As Integer) As INDArray
			Return randomBernoulli(0.5, shape)
		End Function

		Public Shared Function randomBernoulli(ByVal p As Double, ParamArray ByVal shape() As Integer) As INDArray
			Dim ret As INDArray = Nd4j.createUninitialized(shape)
			Nd4j.Executioner.exec(New BernoulliDistribution(ret, p))
			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStreamToFile(File out, InputStream is) throws IOException
		Public Shared Sub writeStreamToFile(ByVal [out] As File, ByVal [is] As Stream)
			Dim b() As SByte = IOUtils.toByteArray([is])
			Using os As Stream = New BufferedOutputStream(New FileStream([out], FileMode.Create, FileAccess.Write))
				os.Write(b, 0, b.Length)
			End Using
		End Sub
	End Class

End Namespace