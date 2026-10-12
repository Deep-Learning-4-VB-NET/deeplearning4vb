Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports TestDataSetIterator = org.nd4j.linalg.dataset.api.iterator.TestDataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg.dataset


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class PreProcessor3D4DTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PreProcessor3D4DTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce3d(ByVal backend As Nd4jBackend)

			Dim myNormalizer As New NormalizerStandardize()
			Dim myMinMaxScaler As New NormalizerMinMaxScaler()

			Dim timeSteps As Integer = 15
			Dim samples As Integer = 100
			'multiplier for the features
			Dim featureScaleA As INDArray = Nd4j.create(New Double() {1, -2, 3}).reshape(ChrW(3), 1)
			Dim featureScaleB As INDArray = Nd4j.create(New Double() {2, 2, 3}).reshape(ChrW(3), 1)

			Dim caseA As New Construct3dDataSet(Me, featureScaleA, timeSteps, samples, 1)
			Dim caseB As New Construct3dDataSet(Me, featureScaleB, timeSteps, samples, 1)

			myNormalizer.fit(caseA.sampleDataSet)
			assertEquals(caseA.expectedMean.castTo(DataType.FLOAT), myNormalizer.Mean.castTo(DataType.FLOAT))
			assertTrue(Transforms.abs(myNormalizer.Std.div(caseA.expectedStd).sub(1)).maxNumber().floatValue() < 0.01)

			myMinMaxScaler.fit(caseB.sampleDataSet)
			assertEquals(caseB.expectedMin.castTo(DataType.FLOAT), myMinMaxScaler.Min.castTo(DataType.FLOAT))
			assertEquals(caseB.expectedMax.castTo(DataType.FLOAT), myMinMaxScaler.Max.castTo(DataType.FLOAT))

			'Same Test with an Iterator, values should be close for std, exact for everything else
			Dim sampleIterA As DataSetIterator = New TestDataSetIterator(caseA.sampleDataSet, 5)
			Dim sampleIterB As DataSetIterator = New TestDataSetIterator(caseB.sampleDataSet, 5)

			myNormalizer.fit(sampleIterA)
			assertEquals(myNormalizer.Mean.castTo(DataType.FLOAT), caseA.expectedMean.castTo(DataType.FLOAT))
			assertTrue(Transforms.abs(myNormalizer.Std.div(caseA.expectedStd).sub(1)).maxNumber().floatValue() < 0.01)

			myMinMaxScaler.fit(sampleIterB)
			assertEquals(myMinMaxScaler.Min.castTo(DataType.FLOAT), caseB.expectedMin.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.Max.castTo(DataType.FLOAT), caseB.expectedMax.castTo(DataType.FLOAT))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce3dMaskLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce3dMaskLabels(ByVal backend As Nd4jBackend)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fitLabel(True)
			Dim myMinMaxScaler As New NormalizerMinMaxScaler()
			myMinMaxScaler.fitLabel(True)

			'generating a dataset with consecutive numbers as feature values. Dataset also has masks
			Dim samples As Integer = 100
			Dim featureScale As INDArray = Nd4j.create(New Double() {1, 2, 10}).reshape(ChrW(3), 1)
			Dim timeStepsU As Integer = 5
			Dim sampleU As New Construct3dDataSet(Me, featureScale, timeStepsU, samples, 1)
			Dim timeStepsV As Integer = 3
			Dim sampleV As New Construct3dDataSet(Me, featureScale, timeStepsV, samples, sampleU.newOrigin)
			Dim dataSetList As IList(Of DataSet) = New List(Of DataSet)()
			dataSetList.Add(sampleU.sampleDataSet)
			dataSetList.Add(sampleV.sampleDataSet)

			Dim fullDataSetA As DataSet = DataSet.merge(dataSetList)
			Dim fullDataSetAA As DataSet = fullDataSetA.copy()
			'This should be the same datasets as above without a mask
			Dim fullDataSetNoMask As New Construct3dDataSet(Me, featureScale, timeStepsU + timeStepsV, samples, 1)

			'preprocessors - label and feature values are the same
			myNormalizer.fit(fullDataSetA)
			assertEquals(myNormalizer.Mean.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMean.castTo(DataType.FLOAT))
			assertEquals(myNormalizer.Std.castTo(DataType.FLOAT), fullDataSetNoMask.expectedStd.castTo(DataType.FLOAT))
			assertEquals(myNormalizer.LabelMean.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMean.castTo(DataType.FLOAT))
			assertEquals(myNormalizer.LabelStd.castTo(DataType.FLOAT), fullDataSetNoMask.expectedStd.castTo(DataType.FLOAT))

			myMinMaxScaler.fit(fullDataSetAA)
			assertEquals(myMinMaxScaler.Min.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMin.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.Max.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMax.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.LabelMin.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMin.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.LabelMax.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMax.castTo(DataType.FLOAT))


			'Same Test with an Iterator, values should be close for std, exact for everything else
			Dim sampleIterA As DataSetIterator = New TestDataSetIterator(fullDataSetA, 5)
			Dim sampleIterB As DataSetIterator = New TestDataSetIterator(fullDataSetAA, 5)

			myNormalizer.fit(sampleIterA)
			assertEquals(myNormalizer.Mean.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMean.castTo(DataType.FLOAT))
			assertEquals(myNormalizer.LabelMean.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMean.castTo(DataType.FLOAT))
			Dim diff1 As Double = Transforms.abs(myNormalizer.Std.div(fullDataSetNoMask.expectedStd).sub(1)).maxNumber().doubleValue()
			Dim diff2 As Double = Transforms.abs(myNormalizer.LabelStd.div(fullDataSetNoMask.expectedStd).sub(1)).maxNumber().doubleValue()
			assertTrue(diff1 < 0.01)
			assertTrue(diff2 < 0.01)

			myMinMaxScaler.fit(sampleIterB)
			assertEquals(myMinMaxScaler.Min.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMin.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.Max.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMax.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.LabelMin.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMin.castTo(DataType.FLOAT))
			assertEquals(myMinMaxScaler.LabelMax.castTo(DataType.FLOAT), fullDataSetNoMask.expectedMax.castTo(DataType.FLOAT))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdX(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdX(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {11.10, 22.20, 33.30, 44.40, 55.50, 66.60, 77.70, 88.80, 99.90, 111.00, 122.10, 133.20, 144.30, 155.40, 166.50, 177.60, 188.70, 199.80, 210.90, 222.00, 233.10, 244.20, 255.30, 266.40, 277.50, 288.60, 299.70, 310.80, 321.90, 333.00, 344.10, 355.20, 366.30, 377.40, 388.50, 399.60, 410.70, 421.80, 432.90, 444.00, 455.10, 466.20, 477.30, 488.40, 499.50, 510.60, 521.70, 532.80, 543.90, 555.00, 566.10, 577.20, 588.30, 599.40, 610.50, 621.60, 632.70, 643.80, 654.90, 666.00, 677.10, 688.20, 699.30, 710.40, 721.50, 732.60, 743.70, 754.80, 765.90, 777.00, 788.10, 799.20, 810.30, 821.40, 832.50, 843.60, 854.70, 865.80, 876.90, 888.00, 899.10, 910.20, 921.30, 932.40, 943.50, 954.60, 965.70, 976.80, 987.90, 999.00, 1, &O10.10, 1, &O21.20, 1, &O32.30, 1, &O43.40, 1, &O54.50, 1, &O65.60, 1, &O76.70, 1, &O87.80, 1, &O98.90, 1, 110.00, 1, 121.10, 1, 132.20, 1, 143.30, 1, 154.40, 1, 165.50, 1, 176.60, 1, 187.70, 1, 198.80, 1, 209.90, 1, 221.00, 1, 232.10, 1, 243.20, 1, 254.30, 1, 265.40, 1, 276.50, 1, 287.60, 1, 298.70, 1, 309.80, 1, 320.90, 1, 332.00, 1, 343.10, 1, 354.20, 1, 365.30, 1, 376.40, 1, 387.50, 1, 398.60, 1, 409.70, 1, 420.80, 1, 431.90, 1, 443.00, 1, 454.10, 1, 465.20, 1, 476.30, 1, 487.40, 1, 498.50, 1, 509.60, 1, 520.70, 1, 531.80, 1, 542.90, 1, 554.00, 1, 565.10, 1, 576.20, 1, 587.30, 1, 598.40, 1, 609.50, 1, 620.60, 1, 631.70, 1, 642.80, 1, 653.90, 1, 665.00, 2.10, 4.20, 6.30, 8.40, 10.50, 12.60, 14.70, 16.80, 18.90, 21.00, 23.10, 25.20, 27.30, 29.40, 31.50, 33.60, 35.70, 37.80, 39.90, 42.00, 44.10, 46.20, 48.30, 50.40, 52.50, 54.60, 56.70, 58.80, 60.90, 63.00, 65.10, 67.20, 69.30, 71.40, 73.50, 75.60, 77.70, 79.80, 81.90, 84.00, 86.10, 88.20, 90.30, 92.40, 94.50, 96.60, 98.70, 100.80, 102.90, 105.00, 107.10, 109.20, 111.30, 113.40, 115.50, 117.60, 119.70, 121.80, 123.90, 126.00, 128.10, 130.20, 132.30, 134.40, 136.50, 138.60, 140.70, 142.80, 144.90, 147.00, 149.10, 151.20, 153.30, 155.40, 157.50, 159.60, 161.70, 163.80, 165.90, 168.00, 170.10, 172.20, 174.30, 176.40, 178.50, 180.60, 182.70, 184.80, 186.90, 189.00, 191.10, 193.20, 195.30, 197.40, 199.50, 201.60, 203.70, 205.80, 207.90, 210.00, 212.10, 214.20, 216.30, 218.40, 220.50, 222.60, 224.70, 226.80, 228.90, 231.00, 233.10, 235.20, 237.30, 239.40, 241.50, 243.60, 245.70, 247.80, 249.90, 252.00, 254.10, 256.20, 258.30, 260.40, 262.50, 264.60, 266.70, 268.80, 270.90, 273.00, 275.10, 277.20, 279.30, 281.40, 283.50, 285.60, 287.70, 289.80, 291.90, 294.00, 296.10, 298.20, 300.30, 302.40, 304.50, 306.60, 308.70, 310.80, 312.90, 315.00, 10.00, 20.00, 30.00, 40.00, 50.00, 60.00, 70.00, 80.00, 90.00, 100.00, 110.00, 120.00, 130.00, 140.00, 150.00, 160.00, 170.00, 180.00, 190.00, 200.00, 210.00, 220.00, 230.00, 240.00, 250.00, 260.00, 270.00, 280.00, 290.00, 300.00, 310.00, 320.00, 330.00, 340.00, 350.00, 360.00, 370.00, 380.00, 390.00, 400.00, 410.00, 420.00, 430.00, 440.00, 450.00, 460.00, 470.00, 480.00, 490.00, 500.00, 510.00, 520.00, 530.00, 540.00, 550.00, 560.00, 570.00, 580.00, 590.00, 600.00, 610.00, 620.00, 630.00, 640.00, 650.00, 660.00, 670.00, 680.00, 690.00, 700.00, 710.00, 720.00, 730.00, 740.00, 750.00, 760.00, 770.00, 780.00, 790.00, 800.00, 810.00, 820.00, 830.00, 840.00, 850.00, 860.00, 870.00, 880.00, 890.00, 900.00, 910.00, 920.00, 930.00, 940.00, 950.00, 960.00, 970.00, 980.00, 990.00, 1, &O00.00, 1, &O10.00, 1, &O20.00, 1, &O30.00, 1, &O40.00, 1, &O50.00, 1, &O60.00, 1, &O70.00, 1, &O80.00, 1, &O90.00, 1, 100.00, 1, 110.00, 1, 120.00, 1, 130.00, 1, 140.00, 1, 150.00, 1, 160.00, 1, 170.00, 1, 180.00, 1, 190.00, 1, 200.00, 1, 210.00, 1, 220.00, 1, 230.00, 1, 240.00, 1, 250.00, 1, 260.00, 1, 270.00, 1, 280.00, 1, 290.00, 1, 300.00, 1, 310.00, 1, 320.00, 1, 330.00, 1, 340.00, 1, 350.00, 1, 360.00, 1, 370.00, 1, 380.00, 1, 390.00, 1, 400.00, 1, 410.00, 1, 420.00, 1, 430.00, 1, 440.00, 1, 450.00, 1, 460.00, 1, 470.00, 1, 480.00, 1, 490.00, 1, 500.00, 99.00, 198.00, 297.00, 396.00, 495.00, 594.00, 693.00, 792.00, 891.00, 990.00, 1, &O89.00, 1, 188.00, 1, 287.00, 1, 386.00, 1, 485.00, 1, 584.00, 1, 683.00, 1, 782.00, 1, 881.00, 1, 980.00, 2, &O79.00, 2, 178.00, 2, 277.00, 2, 376.00, 2, 475.00, 2, 574.00, 2, 673.00, 2, 772.00, 2, 871.00, 2, 970.00, 3, &O69.00, 3, 168.00, 3, 267.00, 3, 366.00, 3, 465.00, 3, 564.00, 3, 663.00, 3, 762.00, 3, 861.00, 3, 960.00, 4, &O59.00, 4, 158.00, 4, 257.00, 4, 356.00, 4, 455.00, 4, 554.00, 4, 653.00, 4, 752.00, 4, 851.00, 4, 950.00, 5, &O49.00, 5, 148.00, 5, 247.00, 5, 346.00, 5, 445.00, 5, 544.00, 5, 643.00, 5, 742.00, 5, 841.00, 5, 940.00, 6, &O39.00, 6, 138.00, 6, 237.00, 6, 336.00, 6, 435.00, 6, 534.00, 6, 633.00, 6, 732.00, 6, 831.00, 6, 930.00, 7, &O29.00, 7, 128.00, 7, 227.00, 7, 326.00, 7, 425.00, 7, 524.00, 7, 623.00, 7, 722.00, 7, 821.00, 7, 920.00, 8, &O19.00, 8, 118.00, 8, 217.00, 8, 316.00, 8, 415.00, 8, 514.00, 8, 613.00, 8, 712.00, 8, 811.00, 8, 910.00, 9, &O09.00, 9, 108.00, 9, 207.00, 9, 306.00, 9, 405.00, 9, 504.00, 9, 603.00, 9, 702.00, 9, 801.00, 9, 900.00, 9, 999.00, 10, &O98.00, 10, 197.00, 10, 296.00, 10, 395.00, 10, 494.00, 10, 593.00, 10, 692.00, 10, 791.00, 10, 890.00, 10, 989.00, 11, &O88.00, 11, 187.00, 11, 286.00, 11, 385.00, 11, 484.00, 11, 583.00, 11, 682.00, 11, 781.00, 11, 880.00, 11, 979.00, 12, &O78.00, 12, 177.00, 12, 276.00, 12, 375.00, 12, 474.00, 12, 573.00, 12, 672.00, 12, 771.00, 12, 870.00, 12, 969.00, 13, &O68.00, 13, 167.00, 13, 266.00, 13, 365.00, 13, 464.00, 13, 563.00, 13, 662.00, 13, 761.00, 13, 860.00, 13, 959.00, 14, &O58.00, 14, 157.00, 14, 256.00, 14, 355.00, 14, 454.00, 14, 553.00, 14, 652.00, 14, 751.00, 14, 850.00, 7.16, 14.31, 21.47, 28.62, 35.78, 42.94, 50.09, 57.25, 64.40, 71.56, 78.72, 85.87, 93.03, 100.18, 107.34, 114.50, 121.65, 128.81, 135.96, 143.12, 150.28, 157.43, 164.59, 171.74, 178.90, 186.06, 193.21, 200.37, 207.52, 214.68, 221.84, 228.99, 236.15, 243.30, 250.46, 257.62, 264.77, 271.93, 279.08, 286.24, 293.40, 300.55, 307.71, 314.86, 322.02, 329.18, 336.33, 343.49, 350.64, 357.80, 364.96, 372.11, 379.27, 386.42, 393.58, 400.74, 407.89, 415.05, 422.20, 429.36, 436.52, 443.67, 450.83, 457.98, 465.14, 472.30, 479.45, 486.61, 493.76, 500.92, 508.08, 515.23, 522.39, 529.54, 536.70, 543.86, 551.01, 558.17, 565.32, 572.48, 579.64, 586.79, 593.95, 601.10, 608.26, 615.42, 622.57, 629.73, 636.88, 644.04, 651.20, 658.35, 665.51, 672.66, 679.82, 686.98, 694.13, 701.29, 708.44, 715.60, 722.76, 729.91, 737.07, 744.22, 751.38, 758.54, 765.69, 772.85, 780.00, 787.16, 794.32, 801.47, 808.63, 815.78, 822.94, 830.10, 837.25, 844.41, 851.56, 858.72, 865.88, 873.03, 880.19, 887.34, 894.50, 901.66, 908.81, 915.97, 923.12, 930.28, 937.44, 944.59, 951.75, 958.90, 966.06, 973.22, 980.37, 987.53, 994.68, 1, &O01.84, 1, &O09.00, 1, &O16.15, 1, &O23.31, 1, &O30.46, 1, &O37.62, 1, &O44.78, 1, &O51.93, 1, &O59.09, 1, &O66.24, 1, &O73.40, 9.00, 18.00, 27.00, 36.00, 45.00, 54.00, 63.00, 72.00, 81.00, 90.00, 99.00, 108.00, 117.00, 126.00, 135.00, 144.00, 153.00, 162.00, 171.00, 180.00, 189.00, 198.00, 207.00, 216.00, 225.00, 234.00, 243.00, 252.00, 261.00, 270.00, 279.00, 288.00, 297.00, 306.00, 315.00, 324.00, 333.00, 342.00, 351.00, 360.00, 369.00, 378.00, 387.00, 396.00, 405.00, 414.00, 423.00, 432.00, 441.00, 450.00, 459.00, 468.00, 477.00, 486.00, 495.00, 504.00, 513.00, 522.00, 531.00, 540.00, 549.00, 558.00, 567.00, 576.00, 585.00, 594.00, 603.00, 612.00, 621.00, 630.00, 639.00, 648.00, 657.00, 666.00, 675.00, 684.00, 693.00, 702.00, 711.00, 720.00, 729.00, 738.00, 747.00, 756.00, 765.00, 774.00, 783.00, 792.00, 801.00, 810.00, 819.00, 828.00, 837.00, 846.00, 855.00, 864.00, 873.00, 882.00, 891.00, 900.00, 909.00, 918.00, 927.00, 936.00, 945.00, 954.00, 963.00, 972.00, 981.00, 990.00, 999.00, 1, &O08.00, 1, &O17.00, 1, &O26.00, 1, &O35.00, 1, &O44.00, 1, &O53.00, 1, &O62.00, 1, &O71.00, 1, &O80.00, 1, &O89.00, 1, &O98.00, 1, 107.00, 1, 116.00, 1, 125.00, 1, 134.00, 1, 143.00, 1, 152.00, 1, 161.00, 1, 170.00, 1, 179.00, 1, 188.00, 1, 197.00, 1, 206.00, 1, 215.00, 1, 224.00, 1, 233.00, 1, 242.00, 1, 251.00, 1, 260.00, 1, 269.00, 1, 278.00, 1, 287.00, 1, 296.00, 1, 305.00, 1, 314.00, 1, 323.00, 1, 332.00, 1, 341.00, 1, 350.00}).reshape(ChrW(1), -1)

			Dim templateStd As Single = array.std(1).getFloat(0)

			assertEquals(301.22601, templateStd, 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce4d(ByVal backend As Nd4jBackend)
			Dim imageDataSet As New Construct4dDataSet(Me, 10, 5, 10, 15)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fit(imageDataSet.sampleDataSet)
			assertEquals(imageDataSet.expectedMean, myNormalizer.Mean)

			Dim aat As Single = Transforms.abs(myNormalizer.Std.div(imageDataSet.expectedStd).sub(1)).maxNumber().floatValue()
			Dim abt As Single = myNormalizer.Std.maxNumber().floatValue()
			Dim act As Single = imageDataSet.expectedStd.maxNumber().floatValue()
			Console.WriteLine("ValA: " & aat)
			Console.WriteLine("ValB: " & abt)
			Console.WriteLine("ValC: " & act)
			assertTrue(aat < 0.05)

			Dim myMinMaxScaler As New NormalizerMinMaxScaler()
			myMinMaxScaler.fit(imageDataSet.sampleDataSet)
			assertEquals(imageDataSet.expectedMin, myMinMaxScaler.Min)
			assertEquals(imageDataSet.expectedMax, myMinMaxScaler.Max)

			Dim copyDataSet As DataSet = imageDataSet.sampleDataSet.copy()
			myNormalizer.transform(copyDataSet)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test3dRevertStandardize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test3dRevertStandardize(ByVal backend As Nd4jBackend)
			test3dRevert(New NormalizerStandardize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test3dRevertNormalize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test3dRevertNormalize(ByVal backend As Nd4jBackend)
			test3dRevert(New NormalizerMinMaxScaler())
		End Sub

		Private Sub test3dRevert(ByVal SUT As DataNormalization)
			Dim features As INDArray = Nd4j.rand(New Integer() {5, 2, 10}, 12345).muli(2).addi(1)
			Dim data As New DataSet(features, Nd4j.zeros(5, 1, 10))
			Dim dataCopy As DataSet = data.copy()

			SUT.fit(data)

			SUT.preProcess(data)
			assertNotEquals(data, dataCopy)

			SUT.revert(data)
			assertEquals(dataCopy.Features, data.Features)
			assertEquals(dataCopy.Labels, data.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test3dNinMaxScaling(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test3dNinMaxScaling(ByVal backend As Nd4jBackend)
			Dim values As INDArray = Nd4j.linspace(-10, 10, 100).reshape(ChrW(5), 2, 10)
			Dim data As New DataSet(values, values)

			Dim SUT As New NormalizerMinMaxScaler()
			SUT.fit(data)
			SUT.preProcess(data)

			' Data should now be in a 0-1 range
			Dim min As Single = data.Features.minNumber().floatValue()
			Dim max As Single = data.Features.maxNumber().floatValue()

			assertEquals(0, min, Nd4j.EPS_THRESHOLD)
			assertEquals(1, max, Nd4j.EPS_THRESHOLD)
		End Sub

		Public Class Construct3dDataSet
			Private ReadOnly outerInstance As PreProcessor3D4DTest


	'        
	'           This will return a dataset where the features are consecutive numbers scaled by featureScaler (a column vector)
	'           If more than one sample is specified it will continue the series from the last sample
	'           If origin is not 1, the series will start from the value given
	'            
			Friend sampleDataSet As DataSet
			Friend featureScale As INDArray
			Friend numFeatures, maxN, timeSteps, samples, origin, newOrigin As Integer
			Friend expectedMean, expectedStd, expectedMin, expectedMax As INDArray

			Public Sub New(ByVal outerInstance As PreProcessor3D4DTest, ByVal featureScale As INDArray, ByVal timeSteps As Integer, ByVal samples As Integer, ByVal origin As Integer)
				Me.outerInstance = outerInstance
				Me.featureScale = featureScale
				Me.timeSteps = timeSteps
				Me.samples = samples
				Me.origin = origin

				numFeatures = CInt(featureScale.size(0))
				maxN = samples * timeSteps
				Dim template As INDArray = Nd4j.linspace(origin, origin + timeSteps - 1, timeSteps).reshape(ChrW(1), -1)
				template = Nd4j.concat(0, Nd4j.linspace(origin, origin + timeSteps - 1, timeSteps).reshape(ChrW(1), -1), template)
				template = Nd4j.concat(0, Nd4j.linspace(origin, origin + timeSteps - 1, timeSteps).reshape(ChrW(1), -1), template)
				template.muliColumnVector(featureScale)
				template = template.reshape(ChrW(1), numFeatures, timeSteps)
				Dim featureMatrix As INDArray = template.dup()

				Dim newStart As Integer = origin + timeSteps
				Dim newEnd As Integer
				For i As Integer = 1 To samples - 1
					newEnd = newStart + timeSteps - 1
					template = Nd4j.linspace(newStart, newEnd, timeSteps).reshape(ChrW(1), -1)
					template = Nd4j.concat(0, Nd4j.linspace(newStart, newEnd, timeSteps).reshape(ChrW(1), -1), template)
					template = Nd4j.concat(0, Nd4j.linspace(newStart, newEnd, timeSteps).reshape(ChrW(1), -1), template)
					template.muliColumnVector(featureScale)
					template = template.reshape(ChrW(1), numFeatures, timeSteps)
					newStart = newEnd + 1
					featureMatrix = Nd4j.concat(0, featureMatrix, template)
				Next i
				Dim labelSet As INDArray = featureMatrix.dup()
				Me.newOrigin = newStart
				sampleDataSet = New DataSet(featureMatrix, labelSet)

				'calculating stats
				' The theoretical mean should be the mean of 1,..samples*timesteps
				Dim theoreticalMean As Single = origin - 1 + (samples * timeSteps + 1) / 2.0f
				expectedMean = Nd4j.create(New Double() {theoreticalMean, theoreticalMean, theoreticalMean}, New Long(){1, 3}).castTo(featureScale.dataType())
				expectedMean.muli(featureScale.transpose())

				Dim stdNaturalNums As Single = CSng(Math.Sqrt((samples * samples * timeSteps * timeSteps - 1) \ 12))
				expectedStd = Nd4j.create(New Double() {stdNaturalNums, stdNaturalNums, stdNaturalNums}, New Long(){1, 3}).castTo(Nd4j.defaultFloatingPointType())
				expectedStd.muli(Transforms.abs(featureScale, True).transpose())
				'preprocessors use the population std so divides by n not (n-1)
				expectedStd = expectedStd.dup().muli(Math.Sqrt(maxN)).divi(Math.Sqrt(maxN))

				'min max assumes all scaling values are +ve
				expectedMin = Nd4j.ones(featureScale.dataType(), 3, 1).muliColumnVector(featureScale)
				expectedMax = Nd4j.ones(featureScale.dataType(),3, 1).muli(samples * timeSteps).muliColumnVector(featureScale)
			End Sub

		End Class

		Public Class Construct4dDataSet
			Private ReadOnly outerInstance As PreProcessor3D4DTest


			Friend sampleDataSet As DataSet
			Friend expectedMean, expectedStd, expectedMin, expectedMax As INDArray
			Friend expectedLabelMean, expectedLabelStd, expectedLabelMin, expectedLabelMax As INDArray

			Public Sub New(ByVal outerInstance As PreProcessor3D4DTest, ByVal nExamples As Integer, ByVal nChannels As Integer, ByVal height As Integer, ByVal width As Integer)
				Me.outerInstance = outerInstance
				Nd4j.Random.setSeed(12345)

				Dim allImages As INDArray = Nd4j.rand(New Integer() {nExamples, nChannels, height, width})
				allImages.get(NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).muli(100).addi(200)
				allImages.get(NDArrayIndex.all(), NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()).muli(0.01).subi(10)

				Dim labels As INDArray = Nd4j.linspace(1, nChannels, nChannels).reshape("c"c, nChannels, 1)
				sampleDataSet = New DataSet(allImages, labels)

				expectedMean = allImages.mean(0, 2, 3).reshape(1,allImages.size(1))
				expectedStd = allImages.std(0, 2, 3).reshape(1,allImages.size(1))

				expectedLabelMean = labels.mean(0).reshape(ChrW(1), labels.size(1))
				expectedLabelStd = labels.std(0).reshape(ChrW(1), labels.size(1))

				expectedMin = allImages.min(0, 2, 3).reshape(1,allImages.size(1))
				expectedMax = allImages.max(0, 2, 3).reshape(1,allImages.size(1))
			End Sub
		End Class

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace