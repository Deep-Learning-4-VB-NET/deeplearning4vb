Imports System.Collections.Generic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Join = org.datavec.api.transform.join.Join
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports LocalTransformExecutor = org.datavec.local.transforms.LocalTransformExecutor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.datavec.local.transforms.transform.join




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestJoin
	Public Class TestJoin
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJoinOneToMany_ManyToOne()
		Public Overridable Sub testJoinOneToMany_ManyToOne()

			Dim customerInfoSchema As Schema = (New Schema.Builder()).addColumnLong("customerID").addColumnString("customerName").build()

			Dim purchasesSchema As Schema = (New Schema.Builder()).addColumnLong("purchaseID").addColumnLong("customerID").addColumnDouble("amount").build()

			Dim infoList As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			infoList.Add(New List(Of Writable) From {
				New LongWritable(12345),
				New Text("Customer12345")
			})
			infoList.Add(New List(Of Writable) From {
				New LongWritable(98765),
				New Text("Customer98765")
			})
			infoList.Add(New List(Of Writable) From {
				New LongWritable(50000),
				New Text("Customer50000")
			})

			Dim purchaseList As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			purchaseList.Add(New List(Of Writable) From {
				New LongWritable(1000000),
				New LongWritable(12345),
				New DoubleWritable(10.00)
			})
			purchaseList.Add(New List(Of Writable) From {
				New LongWritable(1000001),
				New LongWritable(12345),
				New DoubleWritable(20.00)
			})
			purchaseList.Add(New List(Of Writable) From {
				New LongWritable(1000002),
				New LongWritable(98765),
				New DoubleWritable(30.00)
			})

			Dim join As Join = (New Join.Builder(Join.JoinType.RightOuter)).setJoinColumns("customerID").setSchemas(customerInfoSchema, purchasesSchema).build()

			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {
				New LongWritable(12345),
				New Text("Customer12345"),
				New LongWritable(1000000),
				New DoubleWritable(10.00)
			})
			expected.Add(New List(Of Writable) From {
				New LongWritable(12345),
				New Text("Customer12345"),
				New LongWritable(1000001),
				New DoubleWritable(20.00)
			})
			expected.Add(New List(Of Writable) From {
				New LongWritable(98765),
				New Text("Customer98765"),
				New LongWritable(1000002),
				New DoubleWritable(30.00)
			})



			Dim info As IList(Of IList(Of Writable)) = (infoList)
			Dim purchases As IList(Of IList(Of Writable)) = (purchaseList)

			Dim joined As IList(Of IList(Of Writable)) = LocalTransformExecutor.executeJoin(join, info, purchases)
			Dim joinedList As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(joined)
			'Sort by order ID (column 3, index 2)
			joinedList.Sort(Function(o1, o2) Long.compare(o1.get(2).toLong(), o2.get(2).toLong()))
			assertEquals(expected, joinedList)

			assertEquals(3, joinedList.Count)

			Dim expectedColNames As IList(Of String) = New List(Of String) From {"customerID", "customerName", "purchaseID", "amount"}
			assertEquals(expectedColNames, join.OutputSchema.getColumnNames())

			Dim expectedColTypes As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.Long, ColumnType.String, ColumnType.Long, ColumnType.Double}
			assertEquals(expectedColTypes, join.OutputSchema.getColumnTypes())


			'Test Many to one: same thing, but swap the order...
			Dim join2 As Join = (New Join.Builder(Join.JoinType.LeftOuter)).setJoinColumns("customerID").setSchemas(purchasesSchema, customerInfoSchema).build()

			Dim expectedManyToOne As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expectedManyToOne.Add(New List(Of Writable) From {Of Writable})
			expectedManyToOne.Add(New List(Of Writable) From {Of Writable})
			expectedManyToOne.Add(New List(Of Writable) From {Of Writable})

			Dim joined2 As IList(Of IList(Of Writable)) = LocalTransformExecutor.executeJoin(join2, purchases, info)
			Dim joinedList2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(joined2)
			'Sort by order ID (column 0)
			joinedList2.Sort(Function(o1, o2) Long.compare(o1.get(0).toLong(), o2.get(0).toLong()))
			assertEquals(3, joinedList2.Count)

			assertEquals(expectedManyToOne, joinedList2)

			Dim expectedColNames2 As IList(Of String) = New List(Of String) From {"purchaseID", "customerID", "amount", "customerName"}
			assertEquals(expectedColNames2, join2.OutputSchema.getColumnNames())

			Dim expectedColTypes2 As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.Long, ColumnType.Long, ColumnType.Double, ColumnType.String}
			assertEquals(expectedColTypes2, join2.OutputSchema.getColumnTypes())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJoinManyToMany()
		Public Overridable Sub testJoinManyToMany()
			Dim schema1 As Schema = (New Schema.Builder()).addColumnLong("id").addColumnCategorical("category", java.util.Arrays.asList("cat0", "cat1", "cat2")).build()

			Dim schema2 As Schema = (New Schema.Builder()).addColumnLong("otherId").addColumnCategorical("otherCategory", java.util.Arrays.asList("cat0", "cat1", "cat2")).build()

			Dim first As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			first.Add(New List(Of Writable) From {Of Writable})
			first.Add(New List(Of Writable) From {Of Writable})
			first.Add(New List(Of Writable) From {Of Writable})

			Dim second As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			second.Add(New List(Of Writable) From {Of Writable})
			second.Add(New List(Of Writable) From {Of Writable})
			second.Add(New List(Of Writable) From {Of Writable})



			Dim expOuterJoin As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})
			expOuterJoin.Add(New List(Of Writable) From {Of Writable})

			Dim expLeftJoin As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expLeftJoin.Add(New List(Of Writable) From {Of Writable})
			expLeftJoin.Add(New List(Of Writable) From {Of Writable})
			expLeftJoin.Add(New List(Of Writable) From {Of Writable})
			expLeftJoin.Add(New List(Of Writable) From {Of Writable})
			expLeftJoin.Add(New List(Of Writable) From {Of Writable})


			Dim expRightJoin As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expRightJoin.Add(New List(Of Writable) From {Of Writable})
			expRightJoin.Add(New List(Of Writable) From {Of Writable})
			expRightJoin.Add(New List(Of Writable) From {Of Writable})
			expRightJoin.Add(New List(Of Writable) From {Of Writable})
			expRightJoin.Add(New List(Of Writable) From {Of Writable})

			Dim expInnerJoin As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expInnerJoin.Add(New List(Of Writable) From {Of Writable})
			expInnerJoin.Add(New List(Of Writable) From {Of Writable})
			expInnerJoin.Add(New List(Of Writable) From {Of Writable})
			expInnerJoin.Add(New List(Of Writable) From {Of Writable})

			Dim firstRDD As IList(Of IList(Of Writable)) = (first)
			Dim secondRDD As IList(Of IList(Of Writable)) = (second)

			Dim count As Integer = 0
			For Each jt As Join.JoinType In System.Enum.GetValues(GetType(Join.JoinType))
				Dim join As Join = (New Join.Builder(jt)).setJoinColumnsLeft("category").setJoinColumnsRight("otherCategory").setSchemas(schema1, schema2).build()
				Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(LocalTransformExecutor.executeJoin(join, firstRDD, secondRDD))

				'Sort output by column 0, then column 1, then column 2 for comparison to expected...
				[out].Sort(Function(o1, o2)
				Dim w1 As Writable = o1.get(0)
				Dim w2 As Writable = o2.get(0)
				If TypeOf w1 Is NullWritable Then
					Return 1
				ElseIf TypeOf w2 Is NullWritable Then
					Return -1
				End If
				Dim c As Integer = Long.compare(w1.toLong(), w2.toLong())
				If c <> 0 Then
					Return c
				End If
				c = String.CompareOrdinal(o1.get(1).ToString(), o2.get(1).ToString())
				If c <> 0 Then
					Return c
				End If
				w1 = o1.get(2)
				w2 = o2.get(2)
				If TypeOf w1 Is NullWritable Then
					Return 1
				ElseIf TypeOf w2 Is NullWritable Then
					Return -1
				End If
				Return Long.compare(w1.toLong(), w2.toLong())
				End Function)

				Select Case jt
					Case Join.JoinType.Inner
						assertEquals(expInnerJoin, [out])
					Case Join.JoinType.LeftOuter
						assertEquals(expLeftJoin, [out])
					Case Join.JoinType.RightOuter
						assertEquals(expRightJoin, [out])
					Case Join.JoinType.FullOuter
						assertEquals(expOuterJoin, [out])
				End Select
				count += 1
			Next jt

			assertEquals(4, count)
		End Sub

	End Class

End Namespace