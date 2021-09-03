Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports lombok

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

Namespace org.nd4j.parameterserver.model


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor @AllArgsConstructor public class SubscriberState implements java.io.Serializable, Comparable<SubscriberState>
	<Serializable>
	Public Class SubscriberState
		Implements IComparable(Of SubscriberState)

		Private isMaster As Boolean
		Private serverState As String
		Private totalUpdates As Integer
		Private streamId As Integer
		Private connectionInfo As String
		Private parameterUpdaterStatus As IDictionary(Of String, Number)
		Private isAsync As Boolean
		Private isReady As Boolean



		''' <summary>
		''' Returns an empty subscriber state
		''' with -1 as total updates, master as false
		''' and server state as empty </summary>
		''' <returns> an empty subscriber state </returns>
		Public Shared Function empty() As SubscriberState
			Dim map As val = New ConcurrentDictionary(Of String, Number)()
			Return SubscriberState.builder().serverState("empty").streamId(-1).parameterUpdaterStatus(map).totalUpdates(-1).isMaster(False).build()
		End Function



		''' <summary>
		''' Write the subscriber state to the given <seealso cref="DataInput"/>
		''' in the order of:
		''' isMaster
		''' serverState
		''' totalUpdates
		''' streamId </summary>
		''' <param name="dataOutput"> the data output to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(java.io.DataOutput dataOutput) throws java.io.IOException
		Public Overridable Sub write(ByVal dataOutput As DataOutput)
			dataOutput.writeBoolean(isMaster)
			dataOutput.writeUTF(serverState)
			dataOutput.writeInt(totalUpdates)
			dataOutput.writeInt(streamId)

		End Sub

		''' <summary>
		''' Read the subscriber state to the given <seealso cref="DataInput"/>
		''' in the order of:
		''' isMaster
		''' serverState
		''' totalUpdates
		''' streamId </summary>
		''' <param name="dataInput"> the data output to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static SubscriberState read(java.io.DataInput dataInput) throws java.io.IOException
		Public Shared Function read(ByVal dataInput As DataInput) As SubscriberState
			Return SubscriberState.builder().isMaster(dataInput.readBoolean()).serverState(dataInput.readUTF()).totalUpdates(dataInput.readInt()).streamId(dataInput.readInt()).build()
		End Function


		''' <summary>
		''' Return the server opType (master or slave) </summary>
		''' <returns> the server opType </returns>
		Public Overridable Function serverType() As String
			Return If(isMaster, "master", "slave")
		End Function


		''' <summary>
		''' Compares this object with the specified object for order.  Returns a
		''' negative integer, zero, or a positive integer as this object is less
		''' than, equal to, or greater than the specified object.
		''' <para>
		''' </para>
		''' <para>The implementor must ensure <tt>sgn(x.compareTo(y)) ==
		''' -sgn(y.compareTo(x))</tt> for all <tt>x</tt> and <tt>y</tt>.  (This
		''' implies that <tt>x.compareTo(y)</tt> must throw an exception iff
		''' <tt>y.compareTo(x)</tt> throws an exception.)
		''' </para>
		''' <para>
		''' </para>
		''' <para>The implementor must also ensure that the relation is transitive:
		''' <tt>(x.compareTo(y)&gt;0 &amp;&amp; y.compareTo(z)&gt;0)</tt> implies
		''' <tt>x.compareTo(z)&gt;0</tt>.
		''' </para>
		''' <para>
		''' </para>
		''' <para>Finally, the implementor must ensure that <tt>x.compareTo(y)==0</tt>
		''' implies that <tt>sgn(x.compareTo(z)) == sgn(y.compareTo(z))</tt>, for
		''' all <tt>z</tt>.
		''' </para>
		''' <para>
		''' </para>
		''' <para>It is strongly recommended, but <i>not</i> strictly required that
		''' <tt>(x.compareTo(y)==0) == (x.equals(y))</tt>.  Generally speaking, any
		''' class that implements the <tt>Comparable</tt> interface and violates
		''' this condition should clearly indicate this fact.  The recommended
		''' language is "Note: this class has a natural ordering that is
		''' inconsistent with equals."
		''' </para>
		''' <para>
		''' </para>
		''' <para>In the foregoing description, the notation
		''' <tt>sgn(</tt><i>expression</i><tt>)</tt> designates the mathematical
		''' <i>signum</i> function, which is defined to return one of <tt>-1</tt>,
		''' <tt>0</tt>, or <tt>1</tt> according to whether the value of
		''' <i>expression</i> is negative, zero or positive.
		''' 
		''' </para>
		''' </summary>
		''' <param name="o"> the object to be compared. </param>
		''' <returns> a negative integer, zero, or a positive integer as this object
		''' is less than, equal to, or greater than the specified object. </returns>
		''' <exception cref="NullPointerException"> if the specified object is null </exception>
		''' <exception cref="ClassCastException">   if the specified object's opType prevents it
		'''                              from being compared to this object. </exception>
		Public Overridable Function CompareTo(ByVal o As SubscriberState) As Integer Implements IComparable(Of SubscriberState).CompareTo
			Return Integer.compare(streamId, o.streamId)
		End Function
	End Class

End Namespace