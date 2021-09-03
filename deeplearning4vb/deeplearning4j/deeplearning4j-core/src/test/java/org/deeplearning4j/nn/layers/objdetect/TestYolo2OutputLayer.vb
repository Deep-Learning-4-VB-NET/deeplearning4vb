Imports System
Imports System.Collections.Generic
Imports System.IO
Imports val = lombok.val
Imports IOUtils = org.apache.commons.io.IOUtils
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports ObjectDetectionRecordReader = org.datavec.image.recordreader.objdetect.ObjectDetectionRecordReader
Imports VocLabelProvider = org.datavec.image.recordreader.objdetect.impl.VocLabelProvider
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.nn.layers.objdetect



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestYolo2OutputLayer extends org.deeplearning4j.BaseDL4JTest
	Public Class TestYolo2OutputLayer
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testYoloActivateScoreBasic()
		Public Overridable Sub testYoloActivateScoreBasic()

			'Note that we expect some NaNs here - 0/0 for example in IOU calculation. This is handled explicitly in the
			'implementation
			'Nd4j.getExecutioner().setProfilingMode(OpExecutioner.ProfilingMode.ANY_PANIC);

			Dim mb As Integer = 3
			Dim b As Integer = 4
			Dim c As Integer = 3
			Dim depth As Integer = b * (5 + c)
			Dim w As Integer = 6
			Dim h As Integer = 6

			Dim bbPrior As INDArray = Nd4j.rand(b, 2).muliRowVector(Nd4j.create(New Double(){w, h}))


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.01).list().layer((New ConvolutionLayer.Builder()).nIn(depth).nOut(depth).kernelSize(1,1).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPrior).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim y2impl As org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer)

			Dim input As INDArray = Nd4j.rand(New Integer(){mb, depth, h, w})

			Dim [out] As INDArray = y2impl.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertNotNull([out])
			assertArrayEquals(input.shape(), [out].shape())



			'Check score method (simple)
			Dim labelDepth As Integer = 4 + c
			Dim labels As INDArray = Nd4j.zeros(mb, labelDepth, h, w)
			'put 1 object per minibatch, at positions (0,0), (1,1) etc.
			'Positions for label boxes: (1,1) to (2,2), (2,2) to (4,4) etc
			labels.putScalar(0, 4 + 0, 0, 0, 1)
			labels.putScalar(1, 4 + 1, 1, 1, 1)
			labels.putScalar(2, 4 + 2, 2, 2, 1)

			labels.putScalar(0, 0, 0, 0, 1)
			labels.putScalar(0, 1, 0, 0, 1)
			labels.putScalar(0, 2, 0, 0, 2)
			labels.putScalar(0, 3, 0, 0, 2)

			labels.putScalar(1, 0, 1, 1, 2)
			labels.putScalar(1, 1, 1, 1, 2)
			labels.putScalar(1, 2, 1, 1, 4)
			labels.putScalar(1, 3, 1, 1, 4)

			labels.putScalar(2, 0, 2, 2, 3)
			labels.putScalar(2, 1, 2, 2, 3)
			labels.putScalar(2, 2, 2, 2, 6)
			labels.putScalar(2, 3, 2, 2, 6)

			y2impl.setInput(input, LayerWorkspaceMgr.noWorkspaces())
			y2impl.Labels = labels
			Dim score As Double = y2impl.computeScore(0.0, True, LayerWorkspaceMgr.noWorkspaces())

	'        System.out.println("SCORE: " + score);
			assertTrue(score > 0.0)


			'Finally: test ser/de:
			Dim netLoaded As MultiLayerNetwork = TestUtils.testModelSerialization(net)

			y2impl = DirectCast(netLoaded.getLayer(1), org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer)
			y2impl.setInput(input, LayerWorkspaceMgr.noWorkspaces())
			y2impl.Labels = labels
			Dim score2 As Double = y2impl.computeScore(0.0, True, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(score, score2, 1e-8)

			'Test computeScoreForExamples:
			Dim scoreArr1 As INDArray = net.scoreExamples(New DataSet(input, labels), False)
			Dim scoreArr2 As INDArray = net.scoreExamples(New DataSet(input, labels), True)
			assertFalse(scoreArr1.Attached)
			assertFalse(scoreArr2.Attached)

			assertArrayEquals(New Long(){mb, 1}, scoreArr1.shape())
			assertArrayEquals(New Long(){mb, 1}, scoreArr2.shape())
			assertNotEquals(scoreArr1, scoreArr2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testYoloActivateSanityCheck()
		Public Overridable Sub testYoloActivateSanityCheck()

			Dim mb As Integer = 3
			Dim b As Integer = 4
			Dim c As Integer = 3
			Dim depth As Integer = b * (5 + c)
			Dim w As Integer = 6
			Dim h As Integer = 6

			Dim bbPrior As INDArray = Nd4j.rand(b, 2).muliRowVector(Nd4j.create(New Double(){w, h}))


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder()).nIn(1).nOut(1).kernelSize(1,1).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPrior).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim y2impl As org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer)

			Dim input As INDArray = Nd4j.rand(New Integer(){mb, depth, h, w})

			Dim [out] As INDArray = y2impl.activate(input, False, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(4, [out].rank())


			'Check values for x/y, confidence: all should be 0 to 1
			Dim out5 As INDArray = [out].reshape("c"c, mb, b, 5+c, h, w)

			Dim predictedXYCenterGrid As INDArray = out5.get(all(), all(), interval(0,2), all(), all())
			Dim predictedWH As INDArray = out5.get(all(), all(), interval(2,4), all(), all()) 'Shape: [mb, B, 2, H, W]
			Dim predictedConf As INDArray = out5.get(all(), all(), point(4), all(), all()) 'Shape: [mb, B, H, W]


			assertTrue(predictedXYCenterGrid.minNumber().doubleValue() >= 0.0)
			assertTrue(predictedXYCenterGrid.maxNumber().doubleValue() <= 1.0)
			assertTrue(predictedWH.minNumber().doubleValue() >= 0.0)
			assertTrue(predictedConf.minNumber().doubleValue() >= 0.0)
			assertTrue(predictedConf.maxNumber().doubleValue() <= 1.0)


			'Check classes:
			Dim probs As INDArray = out5.get(all(), all(), interval(5, 5+c), all(), all()) 'Shape: [minibatch, C, H, W]
			assertTrue(probs.minNumber().doubleValue() >= 0.0)
			assertTrue(probs.maxNumber().doubleValue() <= 1.0)

			Dim probsSum As INDArray = probs.sum(2)
			assertEquals(1.0, probsSum.minNumber().doubleValue(), 1e-6)
			assertEquals(1.0, probsSum.maxNumber().doubleValue(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIOUCalc(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIOUCalc(ByVal tempDir As Path)

			Dim is1 As Stream = (New ClassPathResource("yolo/VOC_SingleImage/JPEGImages/2007_009346.jpg")).InputStream
			Dim is2 As Stream = (New ClassPathResource("yolo/VOC_SingleImage/Annotations/2007_009346.xml")).InputStream

			Dim dir As New File(tempDir.toFile(),"testYoloOverfitting")
			dir.mkdirs()
			Dim jpg As New File(dir, "JPEGImages")
			Dim annot As New File(dir, "Annotations")
			jpg.mkdirs()
			annot.mkdirs()

			Dim imgOut As New File(jpg, "2007_009346.jpg")
			Dim annotationOut As New File(annot, "2007_009346.xml")

			Try
					Using fos As New FileStream(imgOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is1, fos)
					End Using
			Finally
				is1.Close()
			End Try

			Try
					Using fos As New FileStream(annotationOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is2, fos)
					End Using
			Finally
				is2.Close()
			End Try

	'        INDArray bbPriors = Nd4j.create(new double[][]{
	'                {3, 3},
	'                {5, 4}});
			Dim bbPriors As INDArray = Nd4j.create(New Double()(){
				New Double() {3, 3}
			})

			Dim lp As New VocLabelProvider(dir.getPath())
			Dim c As Integer = 20
			Dim depthOut As val = bbPriors.size(0) * (bbPriors.size(0) + c)

			Dim origW As Integer = 500
			Dim origH As Integer = 375
			Dim inputW As Integer = 52
			Dim inputH As Integer = 52

			Dim gridW As Integer = 13
			Dim gridH As Integer = 13

			Dim rr As RecordReader = New ObjectDetectionRecordReader(inputH, inputW, 3, gridH, gridW, lp)
			rr.initialize(New org.datavec.api.Split.FileSplit(jpg))

			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr,1,1,1,True)

			'2 objects here:
			'(60,123) to (220,305)
			'(243,105) to (437,317)

			Dim cx1 As Double = (60+220)/2.0
			Dim cy1 As Double = (123+305)/2.0
			Dim gridNumX1 As Integer = CInt(Math.Truncate(gridW * cx1 / origW))
			Dim gridNumY1 As Integer = CInt(Math.Truncate(gridH * cy1 / origH))

			Dim labelGridBoxX1_tl As Double = gridW * 60.0 / origW
			Dim labelGridBoxY1_tl As Double = gridH * 123.0 / origH
			Dim labelGridBoxX1_br As Double = gridW * 220.0 / origW
			Dim labelGridBoxY1_br As Double = gridH * 305.0 / origH


			Dim cx2 As Double = (243+437)/2.0
			Dim cy2 As Double = (105+317)/2.0
			Dim gridNumX2 As Integer = CInt(Math.Truncate(gridW * cx2 / origW))
			Dim gridNumY2 As Integer = CInt(Math.Truncate(gridH * cy2 / origH))

			Dim labelGridBoxX2_tl As Double = gridW * 243.0 / origW
			Dim labelGridBoxY2_tl As Double = gridH * 105.0 / origH
			Dim labelGridBoxX2_br As Double = gridW * 437.0 / origW
			Dim labelGridBoxY2_br As Double = gridH * 317.0 / origH

			'Check labels
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim labelImgClasses As INDArray = ds.Labels.get(point(0), point(4), all(), all())
			Dim labelX_tl As INDArray = ds.Labels.get(point(0), point(0), all(), all())
			Dim labelY_tl As INDArray = ds.Labels.get(point(0), point(1), all(), all())
			Dim labelX_br As INDArray = ds.Labels.get(point(0), point(2), all(), all())
			Dim labelY_br As INDArray = ds.Labels.get(point(0), point(3), all(), all())

			Dim expLabelImg As INDArray = Nd4j.create(gridH,gridW)
			expLabelImg.putScalar(gridNumY1, gridNumX1, 1.0)
			expLabelImg.putScalar(gridNumY2, gridNumX2, 1.0)

			Dim expX_TL As INDArray = Nd4j.create(gridH, gridW)
			expX_TL.putScalar(gridNumY1, gridNumX1, labelGridBoxX1_tl)
			expX_TL.putScalar(gridNumY2, gridNumX2, labelGridBoxX2_tl)

			Dim expY_TL As INDArray = Nd4j.create(gridH, gridW)
			expY_TL.putScalar(gridNumY1, gridNumX1, labelGridBoxY1_tl)
			expY_TL.putScalar(gridNumY2, gridNumX2, labelGridBoxY2_tl)

			Dim expX_BR As INDArray = Nd4j.create(gridH, gridW)
			expX_BR.putScalar(gridNumY1, gridNumX1, labelGridBoxX1_br)
			expX_BR.putScalar(gridNumY2, gridNumX2, labelGridBoxX2_br)

			Dim expY_BR As INDArray = Nd4j.create(gridH, gridW)
			expY_BR.putScalar(gridNumY1, gridNumX1, labelGridBoxY1_br)
			expY_BR.putScalar(gridNumY2, gridNumX2, labelGridBoxY2_br)


			assertEquals(expLabelImg, labelImgClasses)
			assertEquals(expX_TL, labelX_tl)
			assertEquals(expY_TL, labelY_tl)
			assertEquals(expX_BR, labelX_br)
			assertEquals(expY_BR, labelY_br)


			'Check IOU calculation

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder()).kernelSize(3,3).stride(1,1).nIn(3).nOut(3).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPriors).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Dim ol As org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer)

			Dim m As System.Reflection.MethodInfo = ol.GetType().getDeclaredMethod("calculateIOULabelPredicted", GetType(INDArray), GetType(INDArray), GetType(INDArray), GetType(INDArray), GetType(INDArray), GetType(INDArray))
			m.setAccessible(True)

			Dim labelTL As INDArray = ds.Labels.get(interval(0,1), interval(0,2), all(), all())
			Dim labelBR As INDArray = ds.Labels.get(interval(0,1), interval(2,4), all(), all())

			Dim pw1 As Double = 2.5
			Dim ph1 As Double = 3.5
			Dim pw2 As Double = 4.5
			Dim ph2 As Double = 5.5
			Dim predictedWH As INDArray = Nd4j.create(1, bbPriors.size(0), 2, gridH, gridW)
			predictedWH.putScalar(New Integer(){0, 0, 0, gridNumY1, gridNumX1}, pw1)
			predictedWH.putScalar(New Integer(){0, 0, 1, gridNumY1, gridNumX1}, ph1)
			predictedWH.putScalar(New Integer(){0, 0, 0, gridNumY2, gridNumX2}, pw2)
			predictedWH.putScalar(New Integer(){0, 0, 1, gridNumY2, gridNumX2}, ph2)

			Dim pX1 As Double = 0.6
			Dim pY1 As Double = 0.8
			Dim pX2 As Double = 0.3
			Dim pY2 As Double = 0.4
			Dim predictedXYInGrid As INDArray = Nd4j.create(1, bbPriors.size(0), 2, gridH, gridW)
			predictedXYInGrid.putScalar(New Integer(){0, 0, 0, gridNumY1, gridNumX1}, pX1)
			predictedXYInGrid.putScalar(New Integer(){0, 0, 1, gridNumY1, gridNumX1}, pY1)
			predictedXYInGrid.putScalar(New Integer(){0, 0, 0, gridNumY2, gridNumX2}, pX2)
			predictedXYInGrid.putScalar(New Integer(){0, 0, 1, gridNumY2, gridNumX2}, pY2)

			Dim objectPresentMask As INDArray = labelImgClasses.reshape(labelImgClasses.ordering(), 1, labelImgClasses.size(0), labelImgClasses.size(1)) 'Only 1 class here, so same thing as object present mask...
			objectPresentMask = objectPresentMask.castTo(DataType.BOOL)

			Dim ret As Object = m.invoke(ol, labelTL, labelBR, predictedWH, predictedXYInGrid, objectPresentMask.castTo(DataType.DOUBLE), objectPresentMask)
			Dim fIou As System.Reflection.FieldInfo = ret.GetType().getDeclaredField("iou")
			fIou.setAccessible(True)
			Dim iou As INDArray = DirectCast(fIou.get(ret), INDArray)


			'Calculate IOU for first image object, first BB
			Dim predictedTL_x1 As Double = gridNumX1 + pX1 - 0.5 * pw1
			Dim predictedTL_y1 As Double = gridNumY1 + pY1 - 0.5 * ph1
			Dim predictedBR_x1 As Double = gridNumX1 + pX1 + 0.5 * pw1
			Dim predictedBR_y1 As Double = gridNumY1 + pY1 + 0.5 * ph1

			Dim intersectionX_TL_1 As Double = Math.Max(predictedTL_x1, labelGridBoxX1_tl)
			Dim intersectionY_TL_1 As Double = Math.Max(predictedTL_y1, labelGridBoxY1_tl)
			Dim intersectionX_BR_1 As Double = Math.Min(predictedBR_x1, labelGridBoxX1_br)
			Dim intersectionY_BR_1 As Double = Math.Min(predictedBR_y1, labelGridBoxY1_br)

			Dim intersection1_bb1 As Double = (intersectionX_BR_1 - intersectionX_TL_1) * (intersectionY_BR_1 - intersectionY_TL_1)
			Dim pArea1 As Double = pw1 * ph1
			Dim lArea1 As Double = (labelGridBoxX1_br - labelGridBoxX1_tl) * (labelGridBoxY1_br - labelGridBoxY1_tl)
			Dim unionA1 As Double = pArea1 + lArea1 - intersection1_bb1
			Dim iou1 As Double = intersection1_bb1 / unionA1

			'Calculate IOU for second image object, first BB
			Dim predictedTL_x2 As Double = gridNumX2 + pX2 - 0.5 * pw2
			Dim predictedTL_y2 As Double = gridNumY2 + pY2 - 0.5 * ph2
			Dim predictedBR_x2 As Double = gridNumX2 + pX2 + 0.5 * pw2
			Dim predictedBR_y2 As Double = gridNumY2 + pY2 + 0.5 * ph2

			Dim intersectionX_TL_2 As Double = Math.Max(predictedTL_x2, labelGridBoxX2_tl)
			Dim intersectionY_TL_2 As Double = Math.Max(predictedTL_y2, labelGridBoxY2_tl)
			Dim intersectionX_BR_2 As Double = Math.Min(predictedBR_x2, labelGridBoxX2_br)
			Dim intersectionY_BR_2 As Double = Math.Min(predictedBR_y2, labelGridBoxY2_br)

			Dim intersection1_bb2 As Double = (intersectionX_BR_2 - intersectionX_TL_2) * (intersectionY_BR_2 - intersectionY_TL_2)
			Dim pArea2 As Double = pw2 * ph2
			Dim lArea2 As Double = (labelGridBoxX2_br - labelGridBoxX2_tl) * (labelGridBoxY2_br - labelGridBoxY2_tl)
			Dim unionA2 As Double = pArea2 + lArea2 - intersection1_bb2
			Dim iou2 As Double = intersection1_bb2 / unionA2

			Dim expIOU As INDArray = Nd4j.create(1, bbPriors.size(0), gridH, gridW)
			expIOU.putScalar(New Integer(){0, 0, gridNumY1, gridNumX1}, iou1)
			expIOU.putScalar(New Integer(){0, 0, gridNumY2, gridNumX2}, iou2)

			assertEquals(expIOU, iou)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testYoloOverfitting(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testYoloOverfitting(ByVal tempDir As Path)
			Nd4j.Random.setSeed(12345)

			Dim is1 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/JPEGImages/2007_009346.jpg")).InputStream
			Dim is2 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/Annotations/2007_009346.xml")).InputStream
			Dim is3 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/JPEGImages/2008_003344.jpg")).InputStream
			Dim is4 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/Annotations/2008_003344.xml")).InputStream

			Dim dir As File = tempDir.toFile()
			Dim jpg As New File(dir, "JPEGImages")
			Dim annot As New File(dir, "Annotations")
			jpg.mkdirs()
			annot.mkdirs()

			Dim imgOut As New File(jpg, "2007_009346.jpg")
			Dim annotationOut As New File(annot, "2007_009346.xml")

			Try
					Using fos As New FileStream(imgOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is1, fos)
					End Using
			Finally
				is1.Close()
			End Try

			Try
					Using fos As New FileStream(annotationOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is2, fos)
					End Using
			Finally
				is2.Close()
			End Try

			imgOut = New File(jpg, "2008_003344.jpg")
			annotationOut = New File(annot, "2008_003344.xml")
			Try
					Using fos As New FileStream(imgOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is3, fos)
					End Using
			Finally
				is3.Close()
			End Try

			Try
					Using fos As New FileStream(annotationOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is4, fos)
					End Using
			Finally
				is4.Close()
			End Try

			assertEquals(2, jpg.listFiles().length)
			assertEquals(2, annot.listFiles().length)

			Dim bbPriors As INDArray = Nd4j.create(New Double()(){
				New Double() {2, 2},
				New Double() {5, 5}
			})

			'4x downsampling to 13x13 = 52x52 input images
			'Required channels at output layer: 5B+C, with B=5, C=20 object classes, for VOC
			Dim lp As New VocLabelProvider(dir.getPath())
			Dim h As Integer = 52
			Dim w As Integer = 52
			Dim c As Integer = 3
			Dim origW As Integer = 500
			Dim origH As Integer = 375
			Dim gridW As Integer = 13
			Dim gridH As Integer = 13

			Dim rr As RecordReader = New ObjectDetectionRecordReader(52, 52, 3, gridH, gridW, lp)
			Dim fileSplit As New org.datavec.api.Split.FileSplit(jpg)
			rr.initialize(fileSplit)

			Dim nClasses As Integer = rr.getLabels().Count
			Dim depthOut As val = bbPriors.size(0) * (5 + nClasses)
			' make sure idxCat is not 0 to test DetectedObject.getPredictedClass()
			Dim labels As IList(Of String) = rr.getLabels()
			labels.Add(labels.RemoveAt(labels.IndexOf("cat")))
			Dim idxCat As Integer = labels.Count - 1


			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr,1,1,1,True)
			iter.PreProcessor = New ImagePreProcessingScaler()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).updater(New Adam(2e-3)).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(3).activation(Activation.LEAKYRELU).weightInit(WeightInit.RELU).seed(12345).list().layer((New ConvolutionLayer.Builder()).kernelSize(5,5).stride(2,2).nOut(256).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2,2).stride(2,2).build()).layer((New ConvolutionLayer.Builder()).activation(Activation.IDENTITY).kernelSize(5,5).stride(1,1).nOut(depthOut).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPriors).build()).setInputType(InputType.convolutional(h,w,c)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.setListeners(New ScoreIterationListener(100))

			Dim nEpochs As Integer = 1000
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim uris() As URI = fileSplit.locations()
			If Not uris(0).getPath().contains("2007_009346") Then
				' make sure to get the cat image
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ds = iter.next()
			End If
			assertEquals(1, ds.Features.size(0))
			For i As Integer = 0 To nEpochs
				net.fit(ds)
			Next i

			Dim ol As org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer = DirectCast(net.getLayer(3), org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer)

			Dim [out] As INDArray = net.output(ds.Features)

	'        for( int i=0; i<bbPriors.size(0); i++ ) {
	'            INDArray confidenceEx0 = ol.getConfidenceMatrix(out, 0, i).dup();
	'
	'            System.out.println(confidenceEx0);
	'            System.out.println("\n");
	'        }

			Dim l As IList(Of DetectedObject) = ol.getPredictedObjects([out], 0.5)
			Console.WriteLine("Detected objects: " & l.Count)
			For Each d As DetectedObject In l
				Console.WriteLine(d)
			Next d


			assertEquals(2, l.Count)

			'Expect 2 detected objects:
			'(60,123) to (220,305)
			'(243,105) to (437,317)

			Dim cx1Pixels As Double = (60+220)/2.0
			Dim cy1Pixels As Double = (123+305)/2.0
			Dim cx1 As Double = gridW * cx1Pixels / origW
			Dim cy1 As Double = gridH * cy1Pixels / origH
			Dim wGrid1 As Double = (220.0-60.0)/origW * gridW
			Dim hGrid1 As Double = (305.0-123.0)/origH * gridH

			Dim cx2Pixels As Double = (243+437)/2.0
			Dim cy2Pixels As Double = (105+317)/2.0
			Dim cx2 As Double = gridW * cx2Pixels / origW
			Dim cy2 As Double = gridH * cy2Pixels / origH
			Dim wGrid2 As Double = (437.0-243.0)/origW * gridW
			Dim hGrid2 As Double = (317-105.0)/origH * gridH


			'Sort by X position...
			l.Sort(New ComparatorAnonymousInnerClass(Me))


			Dim o1 As DetectedObject = l(0)
			Dim p1 As Double = o1.getClassPredictions().getDouble(idxCat)
			Dim c1 As Double = o1.getConfidence()
			assertEquals(idxCat, o1.PredictedClass)
			assertTrue(p1 >= 0.85,p1.ToString())
			assertTrue(c1 >= 0.85,c1.ToString())
			assertEquals(cx1, o1.getCenterX(), 0.1)
			assertEquals(cy1, o1.getCenterY(), 0.1)
			assertEquals(wGrid1, o1.getWidth(), 0.2)
			assertEquals(hGrid1, o1.getHeight(), 0.2)


			Dim o2 As DetectedObject = l(1)
			Dim p2 As Double = o2.getClassPredictions().getDouble(idxCat)
			Dim c2 As Double = o2.getConfidence()
			assertEquals(idxCat, o2.PredictedClass)
			assertTrue(p2 >= 0.85,p2.ToString())
			assertTrue(c2 >= 0.85,c2.ToString())
			assertEquals(cx2, o2.getCenterX(), 0.1)
			assertEquals(cy2, o2.getCenterY(), 0.1)
			assertEquals(wGrid2, o2.getWidth(), 0.2)
			assertEquals(hGrid2, o2.getHeight(), 0.2)
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of DetectedObject)

			Private ReadOnly outerInstance As TestYolo2OutputLayer

			Public Sub New(ByVal outerInstance As TestYolo2OutputLayer)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As DetectedObject, ByVal o2 As DetectedObject) As Integer Implements IComparer(Of DetectedObject).Compare
				Return o1.getCenterX().CompareTo(o2.getCenterX())
			End Function
		End Class
	End Class

End Namespace