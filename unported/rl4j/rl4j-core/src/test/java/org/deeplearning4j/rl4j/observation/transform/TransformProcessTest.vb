Imports System
Imports System.Collections.Generic
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.datavec.api.transform
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

Namespace org.deeplearning4j.rl4j.observation.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class TransformProcessTest
	Public Class TransformProcessTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_noChannelNameIsSuppliedToBuild_expect_exception()
		Public Overridable Sub when_noChannelNameIsSuppliedToBuild_expect_exception()
			' Arrange
			assertThrows(GetType(System.ArgumentException),Sub()
			TransformProcess.builder().build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_callingTransformWithNullArg_expect_exception()
		Public Overridable Sub when_callingTransformWithNullArg_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().build("test")
			sut.transform(Nothing, 0, False)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_callingTransformWithEmptyChannelData_expect_exception()
		Public Overridable Sub when_callingTransformWithEmptyChannelData_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().build("test")
			Dim channelsData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			sut.transform(channelsData, 0, False)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_addingNullFilter_expect_nullException()
		Public Overridable Sub when_addingNullFilter_expect_nullException()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().filter(Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_fileteredOut_expect_skippedObservationAndFollowingOperationsSkipped()
		Public Overridable Sub when_fileteredOut_expect_skippedObservationAndFollowingOperationsSkipped()
			' Arrange
			Dim transformOperationMock As New IntegerTransformOperationMock()
			Dim sut As TransformProcess = TransformProcess.builder().filter(New FilterOperationMock(True)).transform("test", transformOperationMock).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 0, False)

			' Assert
			assertTrue(result.Skipped)
			assertFalse(transformOperationMock.isCalled)
		End Sub

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_addingTransformOnNullChannel_expect_nullException()
		Public Overridable Sub when_addingTransformOnNullChannel_expect_nullException()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().transform(Nothing, New IntegerTransformOperationMock())
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_addingTransformWithNullTransform_expect_nullException()
		Public Overridable Sub when_addingTransformWithNullTransform_expect_nullException()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().transform("test", Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_transformIsCalled_expect_channelDataTransformedInSameOrder()
		Public Overridable Sub when_transformIsCalled_expect_channelDataTransformedInSameOrder()
			' Arrange
			Dim sut As TransformProcess = TransformProcess.builder().filter(New FilterOperationMock(False)).transform("test", New IntegerTransformOperationMock()).transform("test", New ToDataSetTransformOperationMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass2(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 0, False)

			' Assert
			assertFalse(result.Skipped)
			assertEquals(-1.0, result.Data.getDouble(0), 0.00001)
		End Sub

		Private Class HashMapAnonymousInnerClass2
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_addingPreProcessOnNullChannel_expect_nullException()
		Public Overridable Sub when_addingPreProcessOnNullChannel_expect_nullException()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().preProcess(Nothing, New DataSetPreProcessorMock())
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_addingPreProcessWithNullTransform_expect_nullException()
		Public Overridable Sub when_addingPreProcessWithNullTransform_expect_nullException()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().transform("test", Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_preProcessIsCalled_expect_channelDataPreProcessedInSameOrder()
		Public Overridable Sub when_preProcessIsCalled_expect_channelDataPreProcessedInSameOrder()
			' Arrange
			Dim sut As TransformProcess = TransformProcess.builder().filter(New FilterOperationMock(False)).transform("test", New IntegerTransformOperationMock()).transform("test", New ToDataSetTransformOperationMock()).preProcess("test", New DataSetPreProcessorMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass3(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 0, False)

			' Assert
			assertFalse(result.Skipped)
			assertEquals(2, result.Data.shape().Length)
			assertEquals(1, result.Data.shape()(0))
			assertEquals(-10.0, result.Data.getDouble(0), 0.00001)
		End Sub

		Private Class HashMapAnonymousInnerClass3
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_transformingNullData_expect_exception()
		Public Overridable Sub when_transformingNullData_expect_exception()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().transform("test", New IntegerTransformOperationMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass4(Me)
			Dim result As Observation = sut.transform(channelsData, 0, False)
			End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass4
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_transformingAndChannelsNotDataSet_expect_exception()
		Public Overridable Sub when_transformingAndChannelsNotDataSet_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().preProcess("test", New DataSetPreProcessorMock()).build("test")
			Dim result As Observation = sut.transform(Nothing, 0, False)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_transformingAndChannelsEmptyDataSet_expect_exception()
		Public Overridable Sub when_transformingAndChannelsEmptyDataSet_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().preProcess("test", New DataSetPreProcessorMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim result As Observation = sut.transform(channelsData, 0, False)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_buildIsCalledWithoutChannelNames_expect_exception()
		Public Overridable Sub when_buildIsCalledWithoutChannelNames_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			TransformProcess.builder().build()
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_buildIsCalledWithNullChannelName_expect_exception()
		Public Overridable Sub when_buildIsCalledWithNullChannelName_expect_exception()
			assertThrows(GetType(System.NullReferenceException),Sub()
			TransformProcess.builder().build(Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_resetIsCalled_expect_resettableAreReset()
		Public Overridable Sub when_resetIsCalled_expect_resettableAreReset()
			' Arrange
			Dim resettableOperation As New ResettableTransformOperationMock()
			Dim sut As TransformProcess = TransformProcess.builder().filter(New FilterOperationMock(False)).transform("test", New IntegerTransformOperationMock()).transform("test", resettableOperation).build("test")

			' Act
			sut.reset()

			' Assert
			assertTrue(resettableOperation.isResetCalled)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildIsCalledAndAllChannelsAreDataSets_expect_observation()
		Public Overridable Sub when_buildIsCalledAndAllChannelsAreDataSets_expect_observation()
			' Arrange
			Dim sut As TransformProcess = TransformProcess.builder().transform("test", New ToDataSetTransformOperationMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass5(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 123, True)

			' Assert
			assertFalse(result.Skipped)

			assertEquals(1.0, result.Data.getDouble(0), 0.00001)
		End Sub

		Private Class HashMapAnonymousInnerClass5
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildIsCalledAndAllChannelsAreINDArrays_expect_observation()
		Public Overridable Sub when_buildIsCalledAndAllChannelsAreINDArrays_expect_observation()
			' Arrange
			Dim sut As TransformProcess = TransformProcess.builder().build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass6(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 123, True)

			' Assert
			assertFalse(result.Skipped)

			assertEquals(1.0, result.Data.getDouble(0), 0.00001)
		End Sub

		Private Class HashMapAnonymousInnerClass6
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", Nd4j.create(New Double() { 1.0 }))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_buildIsCalledAndChannelsNotDataSetsOrINDArrays_expect_exception()
		Public Overridable Sub when_buildIsCalledAndChannelsNotDataSetsOrINDArrays_expect_exception()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass7(Me)
			Dim result As Observation = sut.transform(channelsData, 123, True)
			End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass7
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_channelDataIsNull_expect_exception()
		Public Overridable Sub when_channelDataIsNull_expect_exception()
			assertThrows(GetType(System.NullReferenceException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().transform("test", New IntegerTransformOperationMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass8(Me)
			sut.transform(channelsData, 0, False)
			End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass8
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", Nothing)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_transformAppliedOnChannelNotInMap_expect_exception()
		Public Overridable Sub when_transformAppliedOnChannelNotInMap_expect_exception()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim sut As TransformProcess = TransformProcess.builder().transform("test", New IntegerTransformOperationMock()).build("test")
		   Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass9(Me)
		   sut.transform(channelsData, 0, False)
		   End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass9
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("not-test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_preProcessAppliedOnChannelNotInMap_expect_exception()
		Public Overridable Sub when_preProcessAppliedOnChannelNotInMap_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As TransformProcess = TransformProcess.builder().preProcess("test", New DataSetPreProcessorMock()).build("test")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass10(Me)
			sut.transform(channelsData, 0, False)
			End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass10
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("not-test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_buildContainsChannelNotInMap_expect_exception()
		Public Overridable Sub when_buildContainsChannelNotInMap_expect_exception()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim sut As TransformProcess = TransformProcess.builder().transform("test", New IntegerTransformOperationMock()).build("not-test")
		   Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass11(Me)
		   sut.transform(channelsData, 0, False)
		   End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass11
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_preProcessNotAppliedOnDataSet_expect_exception()
		Public Overridable Sub when_preProcessNotAppliedOnDataSet_expect_exception()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim sut As TransformProcess = TransformProcess.builder().preProcess("test", New DataSetPreProcessorMock()).build("test")
		   Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass12(Me)
		   sut.transform(channelsData, 0, False)
		   End Sub)

		End Sub

		Private Class HashMapAnonymousInnerClass12
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("test", 1)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_transformProcessHaveMultipleChannels_expect_channelsAreCreatedInTheDefinedOrder()
		Public Overridable Sub when_transformProcessHaveMultipleChannels_expect_channelsAreCreatedInTheDefinedOrder()
			' Arrange
			Dim sut As TransformProcess = TransformProcess.builder().build("channel0", "channel1")
			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass13(Me)

			' Act
			Dim result As Observation = sut.transform(channelsData, 0, False)

			' Assert
			assertEquals(2, result.numChannels())
			assertEquals(123.0, result.getChannelData(0).getDouble(0), 0.000001)
			assertEquals(234.0, result.getChannelData(1).getDouble(0), 0.000001)
		End Sub

		Private Class HashMapAnonymousInnerClass13
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TransformProcessTest

			Public Sub New(ByVal outerInstance As TransformProcessTest)
				Me.outerInstance = outerInstance

				Me.put("channel0", Nd4j.create(New Double() { 123.0 }))
				Me.put("channel1", Nd4j.create(New Double() { 234.0 }))
			End Sub

		End Class

		Private Class FilterOperationMock
			Implements FilterOperation

			Friend ReadOnly skipped As Boolean

			Public Sub New(ByVal skipped As Boolean)
				Me.skipped = skipped
			End Sub

			Public Overridable Function isSkipped(ByVal channelsData As IDictionary(Of String, Object), ByVal currentObservationStep As Integer, ByVal isFinalObservation As Boolean) As Boolean Implements FilterOperation.isSkipped
				Return skipped
			End Function
		End Class

		Private Class IntegerTransformOperationMock
			Implements Operation(Of Integer, Integer)

			Public isCalled As Boolean = False

			Public Overridable Function transform(ByVal input As Integer?) As Integer?
				isCalled = True
				Return -input.Value
			End Function
		End Class

		Private Class ToDataSetTransformOperationMock
			Implements Operation(Of Integer, DataSet)

			Public Overridable Function transform(ByVal input As Integer?) As DataSet
				Return New org.nd4j.linalg.dataset.DataSet(Nd4j.create(New Double() { input }), Nothing)
			End Function
		End Class

		Private Class ResettableTransformOperationMock
			Implements Operation(Of Integer, Integer), ResettableOperation

			Friend isResetCalled As Boolean = False

			Public Overridable Function transform(ByVal input As Integer?) As Integer?
				Return input.Value * 10
			End Function

			Public Overridable Sub reset() Implements ResettableOperation.reset
				isResetCalled = True
			End Sub
		End Class

		<Serializable>
		Private Class DataSetPreProcessorMock
			Implements DataSetPreProcessor

			Public Overridable Sub preProcess(ByVal dataSet As DataSet)
				dataSet.Features.muli(10.0)
			End Sub
		End Class
	End Class

End Namespace