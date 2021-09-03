﻿Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports VariableMultiTimeseriesGenerator = org.deeplearning4j.datasets.iterator.tools.VariableMultiTimeseriesGenerator
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Async Multi Data Set Iterator Test") @NativeTag class AsyncMultiDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class AsyncMultiDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Time Series 1") void testVariableTimeSeries1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableTimeSeries1()
			Dim numBatches As Integer = If(IntegrationTests, 1000, 100)
			Dim batchSize As Integer = If(IntegrationTests, 32, 8)
			Dim timeStepsMin As Integer = 10
			Dim timeStepsMax As Integer = If(IntegrationTests, 500, 100)
			Dim valuesPerTimestep As Integer = If(IntegrationTests, 128, 16)
			Dim iterator As val = New VariableMultiTimeseriesGenerator(1192, numBatches, batchSize, valuesPerTimestep, timeStepsMin, timeStepsMax, 10)
			iterator.reset()
			iterator.hasNext()
			Dim amdsi As val = New AsyncMultiDataSetIterator(iterator, 2, True)
			For e As Integer = 0 To 9
				Dim cnt As Integer = 0
				Do While amdsi.hasNext()
					Dim mds As MultiDataSet = amdsi.next()
					' log.info("Features ptr: {}", AtomicAllocator.getInstance().getPointer(mds.getFeatures()[0].data()).address());
					assertEquals(CDbl(cnt), mds.Features(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.25, mds.Labels(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.5, mds.FeaturesMaskArrays(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.75, mds.LabelsMaskArrays(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					cnt += 1
				Loop
				amdsi.reset()
				log.info("Epoch {} finished...", e)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Time Series 2") void testVariableTimeSeries2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableTimeSeries2()
			Dim numBatches As Integer = If(IntegrationTests, 1000, 100)
			Dim batchSize As Integer = If(IntegrationTests, 32, 8)
			Dim timeStepsMin As Integer = 10
			Dim timeStepsMax As Integer = If(IntegrationTests, 500, 100)
			Dim valuesPerTimestep As Integer = If(IntegrationTests, 128, 16)
			Dim iterator As val = New VariableMultiTimeseriesGenerator(1192, numBatches, batchSize, valuesPerTimestep, timeStepsMin, timeStepsMax, 10)
			For e As Integer = 0 To 9
				iterator.reset()
				iterator.hasNext()
				Dim amdsi As val = New AsyncMultiDataSetIterator(iterator, 2, True)
				Dim cnt As Integer = 0
				Do While amdsi.hasNext()
					Dim mds As MultiDataSet = amdsi.next()
					' log.info("Features ptr: {}", AtomicAllocator.getInstance().getPointer(mds.getFeatures()[0].data()).address());
					assertEquals(CDbl(cnt), mds.Features(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.25, mds.Labels(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.5, mds.FeaturesMaskArrays(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.75, mds.LabelsMaskArrays(0).meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					cnt += 1
				Loop
			Next e
		End Sub
	'    
	'    @Test
	'    public void testResetBug() throws Exception {
	'        // /home/raver119/develop/dl4j-examples/src/main/resources/uci/train/features
	'
	'        SequenceRecordReader trainFeatures = new CSVSequenceRecordReader();
	'        trainFeatures.initialize(new NumberedFileInputSplit("/home/raver119/develop/dl4j-examples/src/main/resources/uci/train/features" + "/%d.csv", 0, 449));
	'        RecordReader trainLabels = new CSVRecordReader();
	'        trainLabels.initialize(new NumberedFileInputSplit("/home/raver119/develop/dl4j-examples/src/main/resources/uci/train/labels" + "/%d.csv", 0, 449));
	'
	'        int miniBatchSize = 10;
	'        int numLabelClasses = 6;
	'        MultiDataSetIterator trainData = new RecordReaderMultiDataSetIterator.Builder(miniBatchSize)
	'                .addSequenceReader("features", trainFeatures)
	'                .addReader("labels", trainLabels)
	'                .addInput("features")
	'                .addOutputOneHot("labels", 0, numLabelClasses)
	'                .build();
	'
	'        //Normalize the training data
	'        MultiDataNormalization normalizer = new MultiNormalizerStandardize();
	'        normalizer.fit(trainData);              //Collect training data statistics
	'        trainData.reset();
	'
	'
	'        SequenceRecordReader testFeatures = new CSVSequenceRecordReader();
	'        testFeatures.initialize(new NumberedFileInputSplit("/home/raver119/develop/dl4j-examples/src/main/resources/uci/test/features" + "/%d.csv", 0, 149));
	'        RecordReader testLabels = new CSVRecordReader();
	'        testLabels.initialize(new NumberedFileInputSplit("/home/raver119/develop/dl4j-examples/src/main/resources/uci/test/labels" + "/%d.csv", 0, 149));
	'
	'        MultiDataSetIterator testData = new RecordReaderMultiDataSetIterator.Builder(miniBatchSize)
	'                .addSequenceReader("features", testFeatures)
	'                .addReader("labels", testLabels)
	'                .addInput("features")
	'                .addOutputOneHot("labels", 0, numLabelClasses)
	'                .build();
	'
	'        System.out.println("-------------- HASH 1----------------");
	'        testData.reset();
	'        while(testData.hasNext()){
	'            System.out.println(Arrays.hashCode(testData.next().getFeatures(0).data().asFloat()));
	'        }
	'
	'        System.out.println("-------------- HASH 2 ----------------");
	'        testData.reset();
	'        testData.hasNext();     //***** Remove this (or move to after async creation), and we get expected results *****
	'        val adsi = new AsyncMultiDataSetIterator(testData, 4, true);    //OR remove this (keeping hasNext) and we get expected results
	'        //val adsi = new AsyncShieldMultiDataSetIterator(testData);
	'        while(adsi.hasNext()){
	'            System.out.println(Arrays.hashCode(adsi.next().getFeatures(0).data().asFloat()));
	'        }
	'    }
	'    
	End Class

End Namespace