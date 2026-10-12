Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNull

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

Namespace org.nd4j.common.base

	Public Class TestPreconditions

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreconditions()
		Public Overridable Sub testPreconditions()

			Preconditions.checkArgument(True)
			Try
				Preconditions.checkArgument(False)
			Catch e As System.ArgumentException
				assertNull(e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here", 10)
			Try
				Preconditions.checkArgument(False, "Message %s here", 10)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here %s there", 10, 20)
			Try
				Preconditions.checkArgument(False, "Message %s here %s there", 10, 20)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here %s there %s more", 10, 20, 30)
			Try
				Preconditions.checkArgument(False, "Message %s here %s there %s more", 10, 20, 30)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here", 10L)
			Try
				Preconditions.checkArgument(False, "Message %s here", 10L)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here %s there", 10L, 20L)
			Try
				Preconditions.checkArgument(False, "Message %s here %s there", 10L, 20L)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here %s there %s more", 10L, 20L, 30L)
			Try
				Preconditions.checkArgument(False, "Message %s here %s there %s more", 10L, 20L, 30L)
			Catch e As System.ArgumentException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkArgument(True, "Message %s here %s there %s more", "A", "B", "C")
			Try
				Preconditions.checkArgument(False, "Message %s here %s there %s more", "A", "B", "C")
			Catch e As System.ArgumentException
				assertEquals("Message A here B there C more", e.Message)
			End Try


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreconditionsMalformed()
		Public Overridable Sub testPreconditionsMalformed()

			'No %s:
			Preconditions.checkArgument(True, "This is malformed", "A", "B", "C")
			Try
				Preconditions.checkArgument(False, "This is malformed", "A", "B", "C")
			Catch e As System.ArgumentException
				assertEquals("This is malformed [A,B,C]", e.Message)
			End Try

			'More args than %s:
			Preconditions.checkArgument(True, "This is %s malformed", "A", "B", "C")
			Try
				Preconditions.checkArgument(False, "This is %s malformed", "A", "B", "C")
			Catch e As System.ArgumentException
				assertEquals("This is A malformed [B,C]", e.Message)
			End Try

			'No args
			Preconditions.checkArgument(True, "This is %s %s malformed")
			Try
				Preconditions.checkArgument(False, "This is %s %s malformed")
			Catch e As System.ArgumentException
				assertEquals("This is %s %s malformed", e.Message)
			End Try

			'More %s than args
			Preconditions.checkArgument(True, "This is %s %s malformed", "A")
			Try
				Preconditions.checkArgument(False, "This is %s %s malformed", "A")
			Catch e As System.ArgumentException
				assertEquals("This is A %s malformed", e.Message)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreconditionsState()
		Public Overridable Sub testPreconditionsState()

			Preconditions.checkState(True)
			Try
				Preconditions.checkState(False)
			Catch e As System.InvalidOperationException
				assertNull(e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here", 10)
			Try
				Preconditions.checkState(False, "Message %s here", 10)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here %s there", 10, 20)
			Try
				Preconditions.checkState(False, "Message %s here %s there", 10, 20)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here %s there %s more", 10, 20, 30)
			Try
				Preconditions.checkState(False, "Message %s here %s there %s more", 10, 20, 30)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here", 10L)
			Try
				Preconditions.checkState(False, "Message %s here", 10L)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here %s there", 10L, 20L)
			Try
				Preconditions.checkState(False, "Message %s here %s there", 10L, 20L)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here %s there %s more", 10L, 20L, 30L)
			Try
				Preconditions.checkState(False, "Message %s here %s there %s more", 10L, 20L, 30L)
			Catch e As System.InvalidOperationException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkState(True, "Message %s here %s there %s more", "A", "B", "C")
			Try
				Preconditions.checkState(False, "Message %s here %s there %s more", "A", "B", "C")
			Catch e As System.InvalidOperationException
				assertEquals("Message A here B there C more", e.Message)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreconditionsMalformedState()
		Public Overridable Sub testPreconditionsMalformedState()

			'No %s:
			Preconditions.checkState(True, "This is malformed", "A", "B", "C")
			Try
				Preconditions.checkState(False, "This is malformed", "A", "B", "C")
			Catch e As System.InvalidOperationException
				assertEquals("This is malformed [A,B,C]", e.Message)
			End Try

			'More args than %s:
			Preconditions.checkState(True, "This is %s malformed", "A", "B", "C")
			Try
				Preconditions.checkState(False, "This is %s malformed", "A", "B", "C")
			Catch e As System.InvalidOperationException
				assertEquals("This is A malformed [B,C]", e.Message)
			End Try

			'No args
			Preconditions.checkState(True, "This is %s %s malformed")
			Try
				Preconditions.checkState(False, "This is %s %s malformed")
			Catch e As System.InvalidOperationException
				assertEquals("This is %s %s malformed", e.Message)
			End Try

			'More %s than args
			Preconditions.checkState(True, "This is %s %s malformed", "A")
			Try
				Preconditions.checkState(False, "This is %s %s malformed", "A")
			Catch e As System.InvalidOperationException
				assertEquals("This is A %s malformed", e.Message)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreconditionsNull()
		Public Overridable Sub testPreconditionsNull()

			Preconditions.checkNotNull("")
			Try
				Preconditions.checkNotNull(Nothing)
			Catch e As System.NullReferenceException
				assertNull(e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here", 10)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here", 10)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there", 10, 20)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there", 10, 20)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there %s more", 10, 20, 30)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", 10, 20, 30)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here", 10L)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here", 10L)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there", 10L, 20L)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there", 10L, 20L)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here 20 there", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there %s more", 10L, 20L, 30L)
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", 10L, 20L, 30L)
			Catch e As System.NullReferenceException
				assertEquals("Message 10 here 20 there 30 more", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there %s more", "A", "B", "C")
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", "A", "B", "C")
			Catch e As System.NullReferenceException
				assertEquals("Message A here B there C more", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there %s more", New Integer(){0, 1}, New Double(){2.0, 3.0}, New Boolean(){True, False})
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", New Integer(){0, 1}, New Double(){2.0, 3.0}, New Boolean(){True, False})
			Catch e As System.NullReferenceException
				assertEquals("Message [0, 1] here [2.0, 3.0] there [true, false] more", e.Message)
			End Try

			Preconditions.checkNotNull("", "Message %s here %s there", New String(){"A", "B"}, New Object(){1.0, "C"})
			Try
				Preconditions.checkNotNull(Nothing, "Message %s here %s there", New String(){"A", "B"}, New Object(){1.0, "C"})
			Catch e As System.NullReferenceException
				assertEquals("Message [A, B] here [1.0, C] there", e.Message)
			End Try
		End Sub

	End Class

End Namespace