Imports System
Imports System.Collections.Generic
Imports System.IO
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports AbstractSameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.AbstractSameDiffLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports ConvolutionLayer = org.deeplearning4j.nn.layers.convolution.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer
Imports BatchNormalization = org.deeplearning4j.nn.layers.normalization.BatchNormalization
Imports LocalResponseNormalization = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization
Imports LSTM = org.deeplearning4j.nn.layers.recurrent.LSTM
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda


	Public Class TestUtils

		Public Shared Function testModelSerialization(ByVal net As MultiLayerNetwork) As MultiLayerNetwork

			Dim restored As MultiLayerNetwork
			Try
				Dim baos As New MemoryStream()
				ModelSerializer.writeModel(net, baos, True)
				Dim bytes() As SByte = baos.toByteArray()

				Dim bais As New MemoryStream(bytes)
				restored = ModelSerializer.restoreMultiLayerNetwork(bais, True)

				assertEquals(net.LayerWiseConfigurations, restored.LayerWiseConfigurations)
				assertEquals(net.params(), restored.params())
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			'Also check the MultiLayerConfiguration is serializable (required by Spark etc)
			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			serializeDeserializeJava(conf)

			Return restored
		End Function

		Public Shared Function testModelSerialization(ByVal net As ComputationGraph) As ComputationGraph
			Dim restored As ComputationGraph
			Try
				Dim baos As New MemoryStream()
				ModelSerializer.writeModel(net, baos, True)
				Dim bytes() As SByte = baos.toByteArray()

				Dim bais As New MemoryStream(bytes)
				restored = ModelSerializer.restoreComputationGraph(bais, True)

				assertEquals(net.Configuration, restored.Configuration)
				assertEquals(net.params(), restored.params())
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			'Also check the ComputationGraphConfiguration is serializable (required by Spark etc)
			Dim conf As ComputationGraphConfiguration = net.Configuration
			serializeDeserializeJava(conf)

			Return restored
		End Function

		Private Shared Function serializeDeserializeJava(Of T)(ByVal [object] As T) As T
			Dim bytes() As SByte
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using baos As System.IO.MemoryStream_Output = new System.IO.MemoryStream_Output(), oos As ObjectOutputStream = new ObjectOutputStream(baos)
					New MemoryStream(), oos As New ObjectOutputStream(baos)
						Using baos As New MemoryStream(), oos As ObjectOutputStream
					oos.writeObject([object])
					oos.close()
					bytes = baos.toByteArray()
					End Using
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			Dim [out] As T
			Try
					Using ois As New ObjectInputStream(New MemoryStream(bytes))
					[out] = CType(ois.readObject(), T)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e)
			End Try

			assertEquals([object], [out])
			Return [out]
		End Function

		Public Shared Function randomOneHot(ByVal examples As Long, ByVal nOut As Long) As INDArray
			Return randomOneHot(examples, nOut, New Random(12345))
		End Function

		Public Shared Function randomOneHot(ByVal dataType As DataType, ByVal examples As Long, ByVal nOut As Long) As INDArray
			Return randomOneHot(dataType, examples, nOut, New Random(12345))
		End Function

		Public Shared Function randomOneHot(ByVal examples As Long, ByVal nOut As Long, ByVal rngSeed As Long) As INDArray
			Return randomOneHot(examples, nOut, New Random(rngSeed))
		End Function

		Public Shared Function randomOneHot(ByVal examples As Long, ByVal nOut As Long, ByVal rng As Random) As INDArray
			Return randomOneHot(Nd4j.defaultFloatingPointType(), examples,nOut, rng)
		End Function

		Public Shared Function randomOneHot(ByVal dataType As DataType, ByVal examples As Long, ByVal nOut As Long, ByVal rng As Random) As INDArray
			Dim arr As INDArray = Nd4j.create(dataType, examples, nOut)
			For i As Integer = 0 To examples - 1
				arr.putScalar(i, rng.Next(CInt(nOut)), 1.0)
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

		Public Shared Function getL1Reg(ByVal l As IList(Of Regularization)) As L1Regularization
			For Each r As Regularization In l
				If TypeOf r Is L1Regularization Then
					Return DirectCast(r, L1Regularization)
				End If
			Next r
			Return Nothing
		End Function

		Public Shared Function getL2Reg(ByVal baseLayer As BaseLayer) As L2Regularization
			Return getL2Reg(baseLayer.getRegularization())
		End Function

		Public Shared Function getL2Reg(ByVal l As IList(Of Regularization)) As L2Regularization
			For Each r As Regularization In l
				If TypeOf r Is L2Regularization Then
					Return DirectCast(r, L2Regularization)
				End If
			Next r
			Return Nothing
		End Function

		Public Shared Function getWeightDecayReg(ByVal bl As BaseLayer) As WeightDecay
			Return getWeightDecayReg(bl.getRegularization())
		End Function

		Public Shared Function getWeightDecayReg(ByVal l As IList(Of Regularization)) As WeightDecay
			For Each r As Regularization In l
				If TypeOf r Is WeightDecay Then
					Return DirectCast(r, WeightDecay)
				End If
			Next r
			Return Nothing
		End Function

		Public Shared Function getL1(ByVal layer As BaseLayer) As Double
			Dim l As IList(Of Regularization) = layer.getRegularization()
			Return getL1(l)
		End Function

		Public Shared Function getL1(ByVal l As IList(Of Regularization)) As Double
			Dim l1Reg As L1Regularization = Nothing
			For Each reg As Regularization In l
				If TypeOf reg Is L1Regularization Then
					l1Reg = DirectCast(reg, L1Regularization)
				End If
			Next reg
			assertNotNull(l1Reg)
			Return l1Reg.getL1().valueAt(0,0)
		End Function

		Public Shared Function getL2(ByVal layer As BaseLayer) As Double
			Dim l As IList(Of Regularization) = layer.getRegularization()
			Return getL2(l)
		End Function

		Public Shared Function getL2(ByVal l As IList(Of Regularization)) As Double
			Dim l2Reg As L2Regularization = Nothing
			For Each reg As Regularization In l
				If TypeOf reg Is L2Regularization Then
					l2Reg = DirectCast(reg, L2Regularization)
				End If
			Next reg
			assertNotNull(l2Reg)
			Return l2Reg.getL2().valueAt(0,0)
		End Function

		Public Shared Function getL1(ByVal layer As AbstractSameDiffLayer) As Double
			Return getL1(layer.getRegularization())
		End Function

		Public Shared Function getL2(ByVal layer As AbstractSameDiffLayer) As Double
			Return getL2(layer.getRegularization())
		End Function

		Public Shared Function getWeightDecay(ByVal layer As BaseLayer) As Double
			Return getWeightDecayReg(layer.getRegularization()).getCoeff().valueAt(0,0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void removeHelper(org.deeplearning4j.nn.api.Layer layer) throws Exception
		Public Shared Sub removeHelper(ByVal layer As Layer)
			removeHelpers(New Layer(){layer})
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void removeHelpers(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub removeHelpers(ByVal layers() As Layer)
			For Each l As Layer In layers

				If TypeOf l Is ConvolutionLayer Then
					Dim f1 As System.Reflection.FieldInfo = GetType(ConvolutionLayer).getDeclaredField("helper")
					f1.setAccessible(True)
					f1.set(l, Nothing)
				ElseIf TypeOf l Is SubsamplingLayer Then
					Dim f2 As System.Reflection.FieldInfo = GetType(SubsamplingLayer).getDeclaredField("helper")
					f2.setAccessible(True)
					f2.set(l, Nothing)
				ElseIf TypeOf l Is BatchNormalization Then
					Dim f3 As System.Reflection.FieldInfo = GetType(BatchNormalization).getDeclaredField("helper")
					f3.setAccessible(True)
					f3.set(l, Nothing)
				ElseIf TypeOf l Is LSTM Then
					Dim f4 As System.Reflection.FieldInfo = GetType(LSTM).getDeclaredField("helper")
					f4.setAccessible(True)
					f4.set(l, Nothing)
				ElseIf TypeOf l Is LocalResponseNormalization Then
					Dim f5 As System.Reflection.FieldInfo = GetType(LocalResponseNormalization).getDeclaredField("helper")
					f5.setAccessible(True)
					f5.set(l, Nothing)
				End If


				If l.Helper IsNot Nothing Then
					Throw New System.InvalidOperationException("Did not remove helper for layer: " & l.GetType().Name)
				End If
			Next l
		End Sub

		Public Shared Sub assertHelperPresent(ByVal layer As Layer)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertHelpersPresent(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub assertHelpersPresent(ByVal layers() As Layer)
			For Each l As Layer In layers
				'Don't use instanceof here - there are sub conv subclasses
				If l.GetType() = GetType(ConvolutionLayer) OrElse TypeOf l Is SubsamplingLayer OrElse TypeOf l Is BatchNormalization OrElse TypeOf l Is LSTM Then
					Preconditions.checkNotNull(l.Helper, l.conf().getLayer().getLayerName())
				End If
			Next l
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertHelpersAbsent(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub assertHelpersAbsent(ByVal layers() As Layer)
			For Each l As Layer In layers
				Preconditions.checkState(l.Helper Is Nothing, l.conf().getLayer().getLayerName())
			Next l
		End Sub
	End Class

End Namespace