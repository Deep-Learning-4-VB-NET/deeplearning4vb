Imports System.Collections.Generic
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports OverlappingTimeWindowFunction = org.datavec.api.transform.sequence.window.OverlappingTimeWindowFunction
Imports TimeWindowFunction = org.datavec.api.transform.sequence.window.TimeWindowFunction
Imports WindowFunction = org.datavec.api.transform.sequence.window.WindowFunction
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.datavec.api.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestWindowFunctions extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestWindowFunctions
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeWindowFunction()
		Public Overridable Sub testTimeWindowFunction()

			'Time windowing: 1 second (1000 milliseconds) window

			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 100L),
				New IntWritable(1)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 200L),
				New IntWritable(2)
			})
			'Second window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 1000L),
				New IntWritable(3)
			})
			'Third window: empty
			'Fourth window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3000L),
				New IntWritable(4)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3100L),
				New IntWritable(5)
			})

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()

			Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
			wf.InputSchema = schema

			Dim windows As IList(Of IList(Of IList(Of Writable))) = wf.applyToSequence(sequence)

			assertEquals(4, windows.Count)
			assertEquals(3, windows(0).Count)
			assertEquals(1, windows(1).Count)
			assertEquals(0, windows(2).Count)
			assertEquals(2, windows(3).Count)

			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(sequence(0))
			exp0.Add(sequence(1))
			exp0.Add(sequence(2))
			assertEquals(exp0, windows(0))

			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(sequence(3))
			assertEquals(exp1, windows(1))

			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			assertEquals(exp2, windows(2))

			Dim exp3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp3.Add(sequence(4))
			exp3.Add(sequence(5))
			assertEquals(exp3, windows(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeWindowFunctionExcludeEmpty()
		Public Overridable Sub testTimeWindowFunctionExcludeEmpty()

			'Time windowing: 1 second (1000 milliseconds) window

			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 100L),
				New IntWritable(1)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 200L),
				New IntWritable(2)
			})
			'Second window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 1000L),
				New IntWritable(3)
			})
			'Third window: empty
			'Fourth window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3000L),
				New IntWritable(4)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3100L),
				New IntWritable(5)
			})

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()

			Dim wf As WindowFunction = (New TimeWindowFunction.Builder()).timeColumn("timecolumn").windowSize(1, TimeUnit.SECONDS).excludeEmptyWindows(True).build()

			wf.InputSchema = schema

			Dim windows As IList(Of IList(Of IList(Of Writable))) = wf.applyToSequence(sequence)

			assertEquals(3, windows.Count)
			assertEquals(3, windows(0).Count)
			assertEquals(1, windows(1).Count)
			assertEquals(2, windows(2).Count)

			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(sequence(0))
			exp0.Add(sequence(1))
			exp0.Add(sequence(2))
			assertEquals(exp0, windows(0))

			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(sequence(3))
			assertEquals(exp1, windows(1))

			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp2.Add(sequence(4))
			exp2.Add(sequence(5))
			assertEquals(exp2, windows(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOverlappingTimeWindowFunctionSimple()
		Public Overridable Sub testOverlappingTimeWindowFunctionSimple()
			'Compare Overlapping and standard window functions where the window separation is equal to the window size
			' In this case, we should get exactly the same results from both.
			'Time windowing: 1 second (1000 milliseconds) window

			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 100L),
				New IntWritable(1)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 200L),
				New IntWritable(2)
			})
			'Second window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 1000L),
				New IntWritable(3)
			})
			'Third window: empty
			'Fourth window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3000L),
				New IntWritable(4)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 3100L),
				New IntWritable(5)
			})

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()

			Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
			wf.InputSchema = schema

			Dim wf2 As WindowFunction = New OverlappingTimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS, 1, TimeUnit.SECONDS)
			wf2.InputSchema = schema

			Dim windowsExp As IList(Of IList(Of IList(Of Writable))) = wf.applyToSequence(sequence)
			Dim windowsAct As IList(Of IList(Of IList(Of Writable))) = wf2.applyToSequence(sequence)

			Dim expSizes() As Integer = {3, 1, 0, 2}
			assertEquals(4, windowsExp.Count)
			assertEquals(4, windowsAct.Count)
			For i As Integer = 0 To 3
				assertEquals(expSizes(i), windowsExp(i).Count)
				assertEquals(expSizes(i), windowsAct(i).Count)

				assertEquals(windowsExp(i), windowsAct(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOverlappingTimeWindowFunction()
		Public Overridable Sub testOverlappingTimeWindowFunction()
			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})


			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()
			'Window size: 2 seconds; calculated every 1 second
			Dim wf2 As WindowFunction = New OverlappingTimeWindowFunction("timecolumn", 2, TimeUnit.SECONDS, 1, TimeUnit.SECONDS)
			wf2.InputSchema = schema

			Dim windowsAct As IList(Of IList(Of IList(Of Writable))) = wf2.applyToSequence(sequence)

			'First window: -1000 to 1000
			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			exp0.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			exp0.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			'Second window: 0 to 2000
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			'Third window: 1000 to 3000
			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp2.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			exp2.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			exp2.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			'Fourth window: 2000 to 4000
			Dim exp3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp3.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			'Fifth window: 3000 to 5000
			Dim exp4 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'Sixth window: 4000 to 6000
			Dim exp5 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp5.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})
			'Seventh window: 5000 to 7000
			Dim exp6 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp6.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})

			Dim windowsExp As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable))) From {exp0, exp1, exp2, exp3, exp4, exp5, exp6}

			assertEquals(7, windowsAct.Count)
			For i As Integer = 0 To 6
				Dim exp As IList(Of IList(Of Writable)) = windowsExp(i)
				Dim act As IList(Of IList(Of Writable)) = windowsAct(i)

				assertEquals(exp, act)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOverlappingTimeWindowFunctionExcludeEmpty()
		Public Overridable Sub testOverlappingTimeWindowFunctionExcludeEmpty()
			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})


			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()
			'Window size: 2 seconds; calculated every 1 second
			'        WindowFunction wf2 = new OverlappingTimeWindowFunction("timecolumn",2,TimeUnit.SECONDS,1,TimeUnit.SECONDS);
			Dim wf2 As WindowFunction = (New OverlappingTimeWindowFunction.Builder()).timeColumn("timecolumn").windowSize(2, TimeUnit.SECONDS).windowSeparation(1, TimeUnit.SECONDS).excludeEmptyWindows(True).build()
			wf2.InputSchema = schema

			Dim windowsAct As IList(Of IList(Of IList(Of Writable))) = wf2.applyToSequence(sequence)

			'First window: -1000 to 1000
			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			exp0.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			exp0.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			'Second window: 0 to 2000
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(New List(Of Writable) From {
				New LongWritable(0),
				New IntWritable(0)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(100),
				New IntWritable(1)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(200),
				New IntWritable(2)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			'Third window: 1000 to 3000
			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp2.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New IntWritable(3)
			})
			exp2.Add(New List(Of Writable) From {
				New LongWritable(1500),
				New IntWritable(4)
			})
			exp2.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			'Fourth window: 2000 to 4000
			Dim exp3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp3.Add(New List(Of Writable) From {
				New LongWritable(2000),
				New IntWritable(5)
			})
			'Fifth window: 3000 to 5000 -> Empty: excluded
			'Sixth window: 4000 to 6000
			Dim exp5 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp5.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})
			'Seventh window: 5000 to 7000
			Dim exp6 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp6.Add(New List(Of Writable) From {
				New LongWritable(5000),
				New IntWritable(7)
			})

			Dim windowsExp As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable))) From {exp0, exp1, exp2, exp3, exp5, exp6}

			assertEquals(6, windowsAct.Count)
			For i As Integer = 0 To 5
				Dim exp As IList(Of IList(Of Writable)) = windowsExp(i)
				Dim act As IList(Of IList(Of Writable)) = windowsAct(i)

				assertEquals(exp, act)
			Next i
		End Sub

	End Class

End Namespace